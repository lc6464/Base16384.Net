using System.Diagnostics;
using System.Security.Cryptography;

DirectoryInfo directoryInfo = new("Source");
if (!directoryInfo.Exists) {
	Console.WriteLine("无 Source 文件夹！");
	Environment.Exit(1);
}

var files = directoryInfo.GetFiles();
if (files.Length == 0) {
	Console.WriteLine("Source 文件夹为空！");
	Environment.Exit(2);
}

if (!Directory.Exists("Result")) {
	if (File.Exists("Result")) {
		Console.WriteLine("Result 存在但为文件！");
		Environment.Exit(3);
	}
	Directory.CreateDirectory("Result");
}

foreach (var file in files) {
	FileInfo encodedFileInfo = new(@$"Result\{file.Name}.Encode"),
		decodedFileInfo = new(@$"Result\{file.Name}.Decode");

	Base16384.EncodeFromFileToNewFile(file, encodedFileInfo);
	Base16384.DecodeFromFileToNewFile(encodedFileInfo, decodedFileInfo);

	using var encP = new Process();
	encP.StartInfo.FileName = "base16384.com";
	encP.StartInfo.Arguments = $"-e \"{file.FullName}\" \"{encodedFileInfo.FullName}.Raw\"";
	encP.StartInfo.RedirectStandardOutput = true;
	encP.Start();
	encP.WaitForExit(5000);
	var encS = encP.StandardOutput.ReadToEnd();
	if (!string.IsNullOrWhiteSpace(encS)) {
		Console.WriteLine($"{file.Name} - encR - {encS}");
	} else if (encP.ExitCode != 0) {
		Console.WriteLine($"{file.Name} - encR - {encP.ExitCode}");
	}

	using var decP = new Process();
	decP.StartInfo.FileName = "base16384.com";
	decP.StartInfo.Arguments = $"-d \"{encodedFileInfo.FullName}.Raw\" \"{decodedFileInfo.FullName}.Raw\"";
	decP.StartInfo.RedirectStandardOutput = true;
	decP.Start();
	decP.WaitForExit(5000);
	var decS = decP.StandardOutput.ReadToEnd();
	if (!string.IsNullOrWhiteSpace(decS)) {
		Console.WriteLine($"{file.Name} - decR - {decS}");
	} else if (decP.ExitCode != 0) {
		Console.WriteLine($"{file.Name} - encR - {decP.ExitCode}");
	}

	Console.WriteLine($"{file.Name} - Finished.");
}

Console.WriteLine("\nAll done.");