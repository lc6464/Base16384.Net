using System.Text;
using System.Diagnostics;

var workPath = @"D:\\temp\\Source";
FileInfo sourceFileInfo = new(Path.Combine(workPath, "test.bin"));
FileInfo encodedByCFileInfo = new($"{sourceFileInfo.FullName}.encodedByC");
FileInfo encodedByNetFileInfo = new($"{sourceFileInfo.FullName}.encodedByNet");
FileInfo encodedByCDecodedByCFileInfo = new($"{encodedByCFileInfo.FullName}.decodedByC");
FileInfo encodedByCDecodedByNetFileInfo = new($"{encodedByCFileInfo.FullName}.decodedByNet");
FileInfo encodedByNetDecodedByCFileInfo = new($"{encodedByNetFileInfo.FullName}.decodedByNet");
FileInfo encodedByNetDecodedByNetFileInfo = new($"{encodedByNetFileInfo.FullName}.decodedByNet");

Debug();


return;

void Debug() {
	using (var encodedByCFileStream = new FileStream(encodedByCFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
		Base16384.DecodeToNewFile(encodedByCFileStream, encodedByCDecodedByNetFileInfo);
		CompareFile(encodedByCDecodedByCFileInfo, encodedByCDecodedByNetFileInfo, "decoded: ");
	}
}
// ConvertFromUtf16BEBytesToUtf8Bytes测试ok
void Test1() {
	var reader = new StreamReader(new FileStream(Path.Combine(workPath, "Test1.txt"), FileMode.Open), Encoding.BigEndianUnicode);
	var readString = reader.ReadToEnd();
	var bytes = Base16384.ConvertFromUtf16BEBytesToUtf8Bytes(Encoding.BigEndianUnicode.GetBytes(readString).ToArray());
	reader.Dispose();
	var writer =
		new StreamWriter(new FileStream(Path.Combine(workPath, "Test1out.txt"), FileMode.Create, FileAccess.Write),
			Encoding.UTF8);
	writer.Write(Encoding.UTF8.GetString(bytes));
	writer.Dispose();
}
// ConvertFromUtf16BEBytesToUtf16LEBytes测试ok
void Test2() {
	var reader = new StreamReader(new FileStream(Path.Combine(workPath, "Test2.txt"), FileMode.Open), Encoding.BigEndianUnicode);
	var readString = reader.ReadToEnd();
	var bytes = Base16384.ConvertFromUtf16BEBytesToUtf16LEBytes(Encoding.BigEndianUnicode.GetBytes(readString).ToArray());
	reader.Dispose();
	var outFileStream = new FileStream(Path.Combine(workPath, "Test2out.txt"), FileMode.Create, FileAccess.Write);
	outFileStream.Write(bytes);
	outFileStream.Dispose();
}
// ConvertFromUtf16BEBytesToString测试ok
void Test3() {
	var reader = new StreamReader(new FileStream(Path.Combine(workPath, "Test3.txt"), FileMode.Open, FileAccess.Read), Encoding.BigEndianUnicode);
	Console.WriteLine(reader.ReadToEnd());
	reader.Dispose();
}
// EncodeToNewFile/DecodeToNewFile(Stream, FileInfo) 测试 建议LC测试
void Test4() {
	using (var sourceFileStream = new FileStream(sourceFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
		Base16384.EncodeToNewFile(sourceFileStream, encodedByNetFileInfo);
		CompareFile(encodedByCFileInfo, encodedByNetFileInfo, "encoded: ");
	}
	using (var encodedByNetFileStream = new FileStream(encodedByNetFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
		Base16384.DecodeToNewFile(encodedByNetFileStream, encodedByNetDecodedByNetFileInfo);
		CompareFile(encodedByCDecodedByCFileInfo, encodedByNetDecodedByNetFileInfo, "decoded: ");
	}
}

void CompareFile(FileInfo info1, FileInfo info2, string tips = "") {
	var bytes1 = File.ReadAllBytes(info1.FullName);
	var bytes2 = File.ReadAllBytes(info2.FullName);
	if (bytes1.Length != bytes2.Length) {
		Console.WriteLine($"{tips}两个文件大小不同");
		return;
	}
	var i = 0;
	while (i < bytes1.Length) {
		if (bytes1[i] != bytes2[i]) {
			Console.WriteLine($"{tips}两个文件内容不同");
			return;
		}
		i++;
	}
	Console.WriteLine($"{tips}两个文件相同");
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
