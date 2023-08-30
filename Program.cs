using System.Text;
using System.Diagnostics;


Testing.Test4();


internal static class Testing {
	private const string workPath = @"D:\\temp\\Source";

	private static readonly FileInfo sourceFileInfo = new(Path.Combine(workPath, "test.bin"));
	private static readonly FileInfo encodedByCFileInfo = new($"{sourceFileInfo.FullName}.encodedByC");
	private static readonly FileInfo encodedByNetFileInfo = new($"{sourceFileInfo.FullName}.encodedByNet");
	private static readonly FileInfo encodedByCDecodedByCFileInfo = new($"{encodedByCFileInfo.FullName}.decodedByC");
	private static readonly FileInfo encodedByCDecodedByNetFileInfo = new($"{encodedByCFileInfo.FullName}.decodedByNet");
	private static readonly FileInfo encodedByNetDecodedByCFileInfo = new($"{encodedByNetFileInfo.FullName}.decodedByNet");
	private static readonly FileInfo encodedByNetDecodedByNetFileInfo = new($"{encodedByNetFileInfo.FullName}.decodedByNet");

	// ConvertFromUtf16BEBytesToUtf8Bytes测试ok
	public static void Test1() {
		using var reader = new StreamReader(new FileStream(Path.Combine(workPath, "Test1.txt"), FileMode.Open),
			Encoding.BigEndianUnicode);
		var readString = reader.ReadToEnd();
		var bytes = Base16384.ConvertFromUtf16BEBytesToUtf8Bytes(Encoding.BigEndianUnicode.GetBytes(readString)
			.ToArray());
		using var writer =
			new StreamWriter(new FileStream(Path.Combine(workPath, "Test1out.txt"), FileMode.Create, FileAccess.Write),
				Encoding.UTF8);
		writer.Write(Encoding.UTF8.GetString(bytes));
	}

	// ConvertFromUtf16BEBytesToUtf16LEBytes测试ok
	public static void Test2() {
		using var reader = new StreamReader(new FileStream(Path.Combine(workPath, "Test2.txt"), FileMode.Open),
			Encoding.BigEndianUnicode);
		var readString = reader.ReadToEnd();
		var bytes = Base16384.ConvertFromUtf16BEBytesToUtf16LEBytes(Encoding.BigEndianUnicode.GetBytes(readString)
			.ToArray());
		using var outFileStream =
			new FileStream(Path.Combine(workPath, "Test2out.txt"), FileMode.Create, FileAccess.Write);
		outFileStream.Write(bytes);
	}

	// ConvertFromUtf16BEBytesToString 测试ok
	public static void Test3() {
		using var reader =
			new StreamReader(new FileStream(Path.Combine(workPath, "Test3.txt"), FileMode.Open, FileAccess.Read),
				Encoding.BigEndianUnicode);
		Console.WriteLine(reader.ReadToEnd());
		reader.Dispose();
	}

// EncodeToNewFile/DecodeToNewFile(Stream, FileInfo) 测试 ???
	public static void Test4() {
		using var sourceStream = sourceFileInfo.OpenRead();
		Base16384.EncodeToNewFile(sourceStream, encodedByNetFileInfo);
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo, "encoded: ");

		using var encodedStream = encodedByNetFileInfo.OpenRead();
		encodedStream.Position += 2;
		Base16384.DecodeToNewFile(encodedStream, encodedByNetDecodedByNetFileInfo);
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo, "decoded: ");
	}

// EncodeToNewMemoryStream/DecodeToNewMemoryStream(Stream) 测试 ???
	public static void Test5() {
		using var sourceFileStream = new FileStream(sourceFileInfo.FullName, FileMode.Open, FileAccess.Read);
		sourceFileStream.Position += 2;
		var encodedMemoryStream = Base16384.EncodeToNewMemoryStream(sourceFileStream);
		File.WriteAllBytes(encodedByNetFileInfo.FullName, encodedMemoryStream.ToArray());
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo);

		using var encodedByNetFileStream =
			new FileStream(encodedByNetFileInfo.FullName, FileMode.Open, FileAccess.Read);
		encodedByNetFileStream.Position += 2;
		var decodedMemoryStream = Base16384.EncodeToNewMemoryStream(encodedByNetFileStream);
		File.WriteAllBytes(encodedByNetDecodedByNetFileInfo.FullName, decodedMemoryStream.ToArray());
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo);
	}

// EncodeToNewMemoryStream/DecodeToNewMemoryStream(ReadOnlySpan) 测试
	public static void Test6() { }

// EncodeToNewFile/DecodeToNewFile(ReadOnlySpan, FileInfo) 测试 ???
	public static void Test7() {
		var sourceBytes = new ReadOnlySpan<byte>(File.ReadAllBytes(sourceFileInfo.FullName));
		Base16384.EncodeToNewFile(sourceBytes, encodedByNetFileInfo);
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo);

		using var encodedFileStream = new FileStream(encodedByNetFileInfo.FullName, FileMode.Open, FileAccess.Read);
		encodedFileStream.Position += 2;
		var buffer = new byte[encodedByNetFileInfo.Length - 2];
		_ = encodedFileStream.Read(buffer);
		Base16384.DecodeToNewFile(new ReadOnlySpan<byte>(buffer), encodedByNetDecodedByNetFileInfo);
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo);
	}

// EncodeFromFileToStream/DecodeFromFileToStream(FileInfo,Stream) 测试
	public static void Test8() { }

// EncodeFromFileToNewFile/DecodeFromFileToNewFile(FileInfo,FileInfo) 用不着测试,因为Console项目用到了这两个方法
	public static void Test9() { }

// EncodeToNewMemoryStream/DecodeToNewMemoryStream(Stream stream, Span<byte> buffer, Span<byte> encodingBuffer) 测试
// EncodeToNewMemoryStream/DecodeToNewMemoryStream(ReadOnlySpan<byte> data, Span<byte> encodingBuffer) 测试
	public static void Test10() { }

// Encode/Decode(ReadOnlySpan<byte> data, byte* bufferPtr) OK
	public static unsafe void Test12() {
		var sourceBytes = File.ReadAllBytes(sourceFileInfo.FullName);
		var encodedIntPtr = (byte*)Marshal.AllocHGlobal(sourceBytes.Length * 2);
		var encodedLength = Base16384.Encode(new ReadOnlySpan<byte>(sourceBytes), encodedIntPtr);

		var encodedBytes = new byte[encodedLength];
		Marshal.Copy((IntPtr)encodedIntPtr, encodedBytes, 0, encodedLength);
		//decode
		var decodedIntPtr = (byte*)Marshal.AllocHGlobal(sourceBytes.Length * 2);
		var decodedLength = Base16384.Decode(new ReadOnlySpan<byte>(encodedBytes), decodedIntPtr);

		var decodedBytes = new byte[decodedLength];
		Marshal.Copy((IntPtr)decodedIntPtr, decodedBytes, 0, decodedLength);
		var decodedFileStream = File.OpenWrite(encodedByNetDecodedByNetFileInfo.FullName);
		decodedFileStream.Write(decodedBytes);
		decodedFileStream.Dispose();
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo);
	}

// EncodeToUnmanagedMemory/DecodeToUnmanagedMemory OK
	public static void Test13() {
		var sourceBytes = File.ReadAllBytes(sourceFileInfo.FullName);
		// unsafe了个寂寞
		var encodeUnmanagedBytes = Base16384.EncodeToUnmanagedMemory(new ReadOnlySpan<byte>(sourceBytes));
		var decodeUnmanagedBytes = Base16384.DecodeToUnmanagedMemory(encodeUnmanagedBytes);
		File.WriteAllBytes(encodedByNetDecodedByNetFileInfo.FullName, decodeUnmanagedBytes.ToArray());
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo);
	}

// Decode(ReadOnlySpan<byte> data, ReadOnlySpan<byte> buffer) OK
// Encode(ReadOnlySpan<byte> data) OK
	public static void Test14() {
		var sourceBytes = File.ReadAllBytes(sourceFileInfo.FullName);
		var encodedSpan = Base16384.Encode(new ReadOnlySpan<byte>(sourceBytes));
		var buffer = new ReadOnlySpan<byte>(new byte[encodedSpan.Length * 2]);
		var decodedLength = Base16384.Decode(encodedSpan, buffer);
		using var decodedStream = File.OpenWrite(encodedByNetDecodedByNetFileInfo.FullName);
		decodedStream.Write(buffer.ToArray(), 0, decodedLength);
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo);
	}

	private static void CompareFile(FileSystemInfo info1, FileSystemInfo info2, string tips = "") {
		var bytes1 = File.ReadAllBytes(info1.FullName);
		var bytes2 = File.ReadAllBytes(info2.FullName);
		if (!bytes1.SequenceEqual(bytes2)) {
			Console.WriteLine($"{tips}两个文件不同");
			return;
		}

		Console.WriteLine($"{tips}两个文件相同");
	}
}


internal static class ProcessByC {
	public static void Encode(FileInfo sourceFileInfo, FileInfo encodedFileInfo) {
		using Process process = new();
		process.StartInfo.FileName = "base16384.com";
		process.StartInfo.Arguments = $"-e \"{sourceFileInfo.FullName}\" \"{encodedFileInfo.FullName}\"";
		process.StartInfo.RedirectStandardOutput = true;
		process.Start();
		if (!process.WaitForExit(5000)) {
			Console.WriteLine($"{sourceFileInfo.Name} - 编码超时！");
		}

		var stdOutString = process.StandardOutput.ReadToEnd();
		if (!string.IsNullOrWhiteSpace(stdOutString)) {
			Console.WriteLine($"{sourceFileInfo.Name} - {stdOutString}");
		}
	}

	public static void Decode(FileInfo encodedFileInfo, FileInfo decodedFileInfo) {
		using Process process = new();
		process.StartInfo.FileName = "base16384.com";
		process.StartInfo.Arguments = $"-d \"{encodedFileInfo.FullName}\" \"{decodedFileInfo.FullName}\"";
		process.StartInfo.RedirectStandardOutput = true;
		process.Start();
		if (!process.WaitForExit(5000)) {
			Console.WriteLine($"{encodedFileInfo.Name} - 编码超时！");
		}

		var stdOutString = process.StandardOutput.ReadToEnd();
		if (!string.IsNullOrWhiteSpace(stdOutString)) {
			Console.WriteLine($"{encodedFileInfo.Name} - {stdOutString}");
		}
	}
}