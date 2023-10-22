switch (args.Length)
{
	case 0:
		Console.WriteLine("用法: Base16384.exe <e|d> <input | \"-\"> [output | \"-\"]");
		Console.WriteLine("e: Encode，将指定 文件/目录下的文件编码至 Result 目录下。");
		Console.WriteLine("d: Decode，将 文件/目录下的文件解码至 Result 目录下。");
		Console.WriteLine("source: 源文件路径，可以是文件夹，也可以是文件。");
		Console.WriteLine("out: 输出文件路径，可以是文件夹，也可以是文件。");
		Console.WriteLine("\"-\" 代表 Standard IO。");
		return 4;
	case <= 3:
		switch (args[0]) {
			case "e":
				if (args.Length < 2) {
					Console.WriteLine("未指定文件或目录");
					return 5;
				}
				return Directory.Exists(args[1]) ? EncodeOrDecodeDirectory(args[1], args.Length == 3 ? args[2] : Path.Combine(args[1], "out"), true) : EncodeOrDecodeFromFileToFile(args[1], args.Length == 3 ? args[2] : $"{args[1]}.encoded", true);
			case "d":
				if (args.Length < 2) {
					Console.WriteLine("未指定文件或目录");
					return 5;
				}
				return Directory.Exists(args[1]) ? EncodeOrDecodeDirectory(args[1], args.Length == 3 ? args[2] : Path.Combine(args[1], "out"), false) : EncodeOrDecodeFromFileToFile(args[1], args.Length == 3 ? args[2] : $"{args[1]}.decoded", false);
			default:
				Console.WriteLine("Unknown argument.");
				return 4;
		}
}

Console.WriteLine("Unknown argument.");
return 4;
// mode: true为encode，false为decode
int EncodeOrDecodeDirectory(string inputPath, string outputPath, bool mode) {
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
		FileInfo outFile = new(Path.Combine(outputPath, $"{file.Name}.{(mode ? "en" : "de")}coded"));
		_ = mode ? Base16384.EncodeFromFileToNewFile(file, outFile) : Base16384.DecodeFromFileToNewFile(file, outFile);
		Console.WriteLine($"{file.Name} - Finished.");
	}
	Console.WriteLine("\nAll done.");
	return 0;
}
// mode: true为encode，false为decode
int EncodeOrDecodeFromFileToFile(string inputPath, string outputPath, bool mode) {
	if (Directory.Exists(outputPath)) {
		outputPath = Path.Combine(outputPath, $"{new FileInfo(inputPath).Name}.{(mode ? "en" : "de")}coded");
	}
	
	const string minus = "-";
	bool inputIsStd = inputPath == minus;
	bool outputIsStd = outputPath == minus;
	if (!File.Exists(inputPath) && !inputIsStd) {
		Console.WriteLine($"{inputPath} 文件不存在");
		return 1;
	}
	using var input = inputIsStd ? Console.OpenStandardInput() : new FileInfo(inputPath).OpenRead();
	using var output = outputIsStd ? Console.OpenStandardOutput() : new FileInfo(outputPath).OpenWrite();
	if (!outputIsStd) {
		output.Write(Base16384.Utf16BEPreamble);
	}
	_ = mode ? Base16384.EncodeToStream(input, output) : Base16384.DecodeToStream(input, output);
	if (!outputIsStd) {
		Console.WriteLine($"{outputPath} - Finished.");
	}
	return 0;
}
bool DirectoryExistsButFileNorCreate(string path) {
	if (Directory.Exists(path)) {
		return false;
	}
	if (File.Exists(path)) {
		return true;
	}
	Console.WriteLine($"自动创建{path}目录");
	Directory.CreateDirectory(path);
	return false;
}