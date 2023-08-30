using System.Text;
using System.Diagnostics;
using System.Net;

Testing.Test7();


internal static class Testing {
	private const string workPath = @"D:\temp\Source";

	private static readonly FileInfo sourceFileInfo = new(Path.Combine(workPath, "test.bin"));
	private static readonly FileInfo encodedByCFileInfo = new($"{sourceFileInfo.FullName}.encodedByC");
	private static readonly FileInfo encodedByNetFileInfo = new($"{sourceFileInfo.FullName}.EncodedByNet");
	private static readonly FileInfo encodedByCDecodedByCFileInfo = new($"{encodedByCFileInfo.FullName}.decodedByC");
	private static readonly FileInfo encodedByCDecodedByNetFileInfo = new($"{encodedByCFileInfo.FullName}.DecodedByNet");
	private static readonly FileInfo encodedByNetDecodedByCFileInfo = new($"{encodedByNetFileInfo.FullName}.decodedByC");
	private static readonly FileInfo encodedByNetDecodedByNetFileInfo = new($"{encodedByNetFileInfo.FullName}.DecodedByNet");


	public static void Debug() {
		CompareFile(sourceFileInfo, encodedByCDecodedByCFileInfo);
	}
	// ConvertFromUtf16BEBytesToUtf8Bytes 测试 pass
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
	// ConvertFromUtf16BEBytesToUtf16LEBytes 测试 pass
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
	// ConvertFromUtf16BEBytesToString 测试 pass
	public static void Test3() { 
		using var reader =
			new StreamReader(new FileStream(Path.Combine(workPath, "Test3.txt"), FileMode.Open, FileAccess.Read),
				Encoding.BigEndianUnicode);
		Console.WriteLine(reader.ReadToEnd());
		reader.Dispose();
	}
	// EncodeToNewFile/DecodeToNewFile(Stream, FileInfo) 测试 pass
	public static void Test4() { 
		using var sourceFileStream = sourceFileInfo.OpenRead();
		Base16384.EncodeToNewFile(sourceFileStream, encodedByNetFileInfo);
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo, "Encode");

		using var encodedFileStream = encodedByNetFileInfo.OpenRead();
		encodedFileStream.Position += 2;
		Base16384.DecodeToNewFile(encodedFileStream, encodedByNetDecodedByNetFileInfo);
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo, "Decode");
	}
	// EncodeToNewMemoryStream/DecodeToNewMemoryStream(Stream) 测试 未通过
	public static void Test5() { 
		using var sourceFileStream = sourceFileInfo.OpenRead();
		var encodedMemoryStream = Base16384.EncodeToNewMemoryStream(sourceFileStream);
		 encodedMemoryStream.Position = 0;
		using (var encodeByNetFileStream = encodedByNetFileInfo.Create()) {
			encodedMemoryStream.CopyTo(encodeByNetFileStream);
		}
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo);


		using (var encodedByNetFileStream = encodedByNetFileInfo.OpenRead()) {
			encodedByNetFileStream.Position += 2;
			var decodeMemoryStream = Base16384.EncodeToNewMemoryStream(encodedByNetFileStream);
			decodeMemoryStream.Position = 0;
			using var encodeByNetFileStream = encodedByNetDecodedByNetFileInfo.Create();
			decodeMemoryStream.CopyTo(encodeByNetFileStream);
		}
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo);
	}
	// EncodeToNewMemoryStream/DecodeToNewMemoryStream(ReadOnlySpan) 测试 未通过
	public static void Test6() {
		var sourceBytes = File.ReadAllBytes(sourceFileInfo.FullName);
		using var encodedMemoryStream = Base16384.EncodeToNewMemoryStream(new ReadOnlySpan<byte>(sourceBytes));
		using (var encodeFileStream = File.OpenWrite(encodedByNetFileInfo.FullName))
		{
			encodedMemoryStream.CopyTo(encodeFileStream);
		}
		CompareEncodedFile();
		
		var decodedBytes = File.ReadAllBytes(encodedByNetFileInfo.FullName);
		using var decodedMemoryStream = Base16384.DecodeToNewMemorySteam(new ReadOnlySpan<byte>(decodedBytes, 1, decodedBytes.Length -2));
		using (var decodedFileStream = File.OpenWrite(encodedByNetDecodedByNetFileInfo.FullName)) {
			decodedMemoryStream.CopyTo(decodedFileStream);
		}
		CompareDecodedFile();

	}
	// EncodeToNewFile/DecodeToNewFile(ReadOnlySpan, FileInfo) 测试 未通过
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
	public static void Test8() {
		var encodedStream = File.OpenWrite(encodedByNetFileInfo.FullName);
		Base16384.EncodeFromFileToStream(sourceFileInfo, encodedStream);
		encodedStream.Dispose();
		CompareEncodedFile();

		var decodedStream = File.OpenWrite(encodedByNetDecodedByCFileInfo.FullName);
		Base16384.DecodeFromFileToStream(encodedByNetFileInfo, decodedStream);
		decodedStream.Dispose();
		CompareDecodedFile();
	}
	// EncodeFromFileToNewFile/DecodeFromFileToNewFile(FileInfo,FileInfo) 测试 pass
	public static void Test9() {
		Base16384.EncodeFromFileToNewFile(sourceFileInfo, encodedByNetFileInfo);
		CompareEncodedFile();

		Base16384.DecodeFromFileToNewFile(encodedByNetFileInfo, encodedByNetDecodedByNetFileInfo);
		CompareDecodedFile();
	} 
	// EncodeToNewMemoryStream/DecodeToNewMemoryStream(Stream stream, Span<byte> buffer, Span<byte> encodingBuffer) 测试
    // EncodeToNewMemoryStream/DecodeToNewMemoryStream(ReadOnlySpan<byte> data, Span<byte> encodingBuffer) 测试
	public static void Test10() { }
	// Encode/Decode(ReadOnlySpan<byte> data, byte* bufferPtr) pass
	public static unsafe void Test12() { 
		var sourceBytes = File.ReadAllBytes(sourceFileInfo.FullName);
		var encodedIntPtr = (byte*)Marshal.AllocHGlobal(sourceBytes.Length * 2);
		var encodedLength = Base16384.Encode(new ReadOnlySpan<byte>(sourceBytes), encodedIntPtr);

		var encodedBytes = new byte[encodedLength];
		Marshal.Copy((nint)encodedIntPtr, encodedBytes, 0, encodedLength);
		//decode
		var decodedIntPtr = (byte*)Marshal.AllocHGlobal(sourceBytes.Length * 2);
		var decodedLength = Base16384.Decode(new ReadOnlySpan<byte>(encodedBytes), decodedIntPtr);

		var decodedBytes = new byte[decodedLength];
		Marshal.Copy((nint)decodedIntPtr, decodedBytes, 0, decodedLength);
		var decodedFileStream = File.OpenWrite(encodedByNetDecodedByNetFileInfo.FullName);
		decodedFileStream.Write(decodedBytes);
		decodedFileStream.Dispose();
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo);
	}
	// EncodeToUnmanagedMemory/DecodeToUnmanagedMemory pass
	public static void Test13() { 
		var sourceBytes = File.ReadAllBytes(sourceFileInfo.FullName);
		var encodeUnmanagedBytes = Base16384.EncodeToUnmanagedMemory(new ReadOnlySpan<byte>(sourceBytes));
		var decodeUnmanagedBytes = Base16384.DecodeToUnmanagedMemory(encodeUnmanagedBytes);
		File.WriteAllBytes(encodedByNetDecodedByNetFileInfo.FullName, decodeUnmanagedBytes.ToArray());
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo);
	}

	// Decode(ReadOnlySpan<byte> data, ReadOnlySpan<byte> buffer) pass
	// Encode(ReadOnlySpan<byte> data) pass
	public static void Test14() {
		
		var sourceBytes = File.ReadAllBytes(sourceFileInfo.FullName);
		var encodedSpan = Base16384.Encode(new ReadOnlySpan<byte>(sourceBytes));
		var buffer = new ReadOnlySpan<byte>(new byte[encodedSpan.Length * 2]);
		var decodedLength = Base16384.Decode(encodedSpan, buffer);
		using var decodedStream = File.OpenWrite(encodedByNetDecodedByNetFileInfo.FullName);
		decodedStream.Write(buffer.ToArray(), 0, decodedLength);
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo);
	}

	private static void CompareEncodedFile() =>
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo);

	private static void CompareDecodedFile() =>
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo);

	private static void CompareNetSourceAndDecoded() =>
		CompareFile(sourceFileInfo, encodedByNetDecodedByNetFileInfo);
	
	private static void CompareFile(FileInfo info1, FileInfo info2, string tips = "") {
		if (info1.Length != info2.Length) {
			Console.WriteLine($"{tips}：啊？");
			return;
		}
		Span<byte> buffer1 = new byte[1048576],
			buffer2 = new byte[1048576];

		using var stream1 = info1.OpenRead();
		using var stream2 = info2.OpenRead();

		int readLength;
		while (true) {
			if ((readLength = stream1.Read(buffer1)) == 0) {
				break;
			}
			stream2.Read(buffer2);

			if (!buffer1[..readLength].SequenceEqual(buffer2[..readLength])) {
				Console.WriteLine($"{tips}：啊？？");
				return;
			}
		}
		Console.WriteLine($"{tips}：嗯。");
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