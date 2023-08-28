if (args.Length <= 3) {
	switch (args[0])
	{
		case "-e":
			if (args.Length < 2) {
				Console.WriteLine("未指定文件或目录");
				return 5;
			}
			return Directory.Exists(args[1]) ? EncodeOrDecodeDirectory(args[1], args.Length == 3 ? args[2] : $"{args[1]}\\Result", true) : EncodeOrDecodeFromFileToFile(args[1], args.Length == 3 ? args[2] : $"{args[1]}.Encoded", true);
		case "-d":
			if (args.Length < 2) {
				Console.WriteLine("未指定文件或目录"); return 5;
			}
			return Directory.Exists(args[1]) ? EncodeOrDecodeDirectory(args[1], args.Length == 3 ? args[2] : $"{args[1]}\\Result", false) : EncodeOrDecodeFromFileToFile(args[1], args.Length == 3 ? args[2] : $"{args[1]}.Encoded", false);
		case "e":
			return EncodeOrDecodeDirectory("Source", "Result", true);
		case "d":
			return EncodeOrDecodeDirectory("Encoded", "Result", false);
		default:
			Console.WriteLine("Unknown argument.");
			return 4;
	}
}
Console.WriteLine("Unknown argument.");
return 4;
// mode: true为encode，false为decode
int EncodeOrDecodeDirectory(string inputPath,string outputPath,bool mode) {
	DirectoryInfo directoryInfo = new(inputPath);
	if (!directoryInfo.Exists) {
		Console.WriteLine($"无 {inputPath} 文件夹！");
		return 1;
	}
	var files = directoryInfo.GetFiles();
	if (files.Length == 0) {
		Console.WriteLine($"{inputPath} 文件夹为空！");
		return 2;
	}
	if (DirectoryExistsButFileNorCreate(outputPath)) {
		Console.WriteLine($"{outputPath} 存在但为文件！");
		return 3;
	}

	foreach (var file in files) {
		FileInfo outFile = new(Path.Combine(outputPath, $"{file.Name}.{(mode ? "En" : "De")}coded"));
		_ = mode ? Base16384.EncodeFromFileToNewFile(file, outFile) : Base16384.DecodeFromFileToNewFile(file, outFile);
		Console.WriteLine($"{file.Name} - Finished.");
	}
	Console.WriteLine("\nAll done.");
	return 0;
}
// mode: true为encode，false为decode
int EncodeOrDecodeFromFileToFile(string inputPath, string outputPath, bool mode) {
	if (!File.Exists(inputPath)) {
		Console.WriteLine($"{inputPath} 文件不存在");
		return 1;
	}
	FileInfo file = new(inputPath);
	FileInfo outFile = new(outputPath);
	_ = mode ? Base16384.EncodeFromFileToNewFile(file, outFile) : Base16384.DecodeFromFileToNewFile(file, outFile);
	Console.WriteLine($"{file.Name} - Finished.");
	return 0;
}
bool DirectoryExistsButFileNorCreate(string path) {
	if (Directory.Exists(path)) {
		return false;
	}
	if (File.Exists(path)) {
		return true;
	}
	Directory.CreateDirectory(path);
	return false;
}