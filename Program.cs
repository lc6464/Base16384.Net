using System.Text;
using System.Diagnostics;

Span<byte> buffer = new byte[8388650];
Span<byte> codingBuffer = new byte[Math.Max(Base16384.EncodeLength(Base16384.Buffer0Length), Base16384.DecodeLength(Base16384.Buffer1Length)) + 16]; /*{ // 编码
	DirectoryInfo directoryInfo = new("Source");
	if (!directoryInfo.Exists) {
		Console.WriteLine("无 Source 文件夹！");
		return 1;
	}

	var files = directoryInfo.GetFiles();
	if (files.Length == 0) {
		Console.WriteLine("Source 文件夹为空！");
		return 2;
	}

	if (!Directory.Exists("Result")) {
		if (File.Exists("Result")) {
			Console.WriteLine("Result 存在但为文件！");
			return 3;
		}
		Directory.CreateDirectory("Result");
	}

	foreach (var file in files) {
		FileInfo encodedFileInfo = new(Path.Combine("Result", $"{file.Name}.Encoded"));

		Base16384.EncodeFromFileToNewFile(file, encodedFileInfo, buffer, codingBuffer);
		//ProcessByC.Encode(file, new(Path.Combine("Result", $"{file.Name}")));

		Console.WriteLine($"{file.Name} - Finished.");
	}

	Console.WriteLine("\nEncode all done.");
} { // 解码
	DirectoryInfo directoryInfo = new("Result");
	if (!directoryInfo.Exists) {
		Console.WriteLine("无 Encoded 文件夹！");
		return 1;
	}

	var files = directoryInfo.GetFiles();
	if (files.Length == 0) {
		Console.WriteLine("Encoded 文件夹为空！");
		return 2;
	}

	if (!Directory.Exists("Result")) {
		if (File.Exists("Result")) {
			Console.WriteLine("Result 存在但为文件！");
			return 3;
		}
		Directory.CreateDirectory("Result");
	}

	foreach (var file in files) {
		FileInfo decodedFileInfo = new(Path.Combine("Result", $"{file.Name}.Decoded"));

		Base16384.DecodeFromFileToNewFile(file, decodedFileInfo, buffer, codingBuffer);
		//ProcessByC.Decode(file, new(Path.Combine("Result", $"{file.Name}")));

		Console.WriteLine($"{file.Name} - Finished.");
	}

	Console.WriteLine("\nDecode all done.");
}

foreach (var file in new DirectoryInfo("Source").GetFiles()) {
	file.CopyTo(Path.Combine("Result", file.Name), true);
}
*/

DirectoryInfo directoryInfo = new("Source");

var fileInfo = directoryInfo.GetFiles()[0];

var stream = new MemoryStream();

Base16384.EncodeFromFileToStream(fileInfo, stream, buffer, codingBuffer);

ReadOnlySpan<byte> data = stream.ToArray();

using var file = File.Create(@"D:\LC\Desktop\test.txt");
file.Write(data.ConvertFromUtf16BEBytesToUtf8BOMBytes());

return 0;



internal static class ProcessByC {
	public static void Encode(FileInfo sourceFileInfo, FileInfo encodedFileInfo) {
		using Process process = new();
		process.StartInfo.FileName = "base16384.com";
		process.StartInfo.Arguments = $"-e \"{sourceFileInfo.FullName}\" \"{encodedFileInfo.FullName}.encodedByC\"";
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
		process.StartInfo.Arguments = $"-d \"{encodedFileInfo.FullName}\" \"{decodedFileInfo.FullName}.decodedByC\"";
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