// 新的解析方式：Base16384.Net.exe <"e" | "d"> <source | "-"> [out | "-"]
/*
错误代码：
1. 无法解析命令行参数
2. 无法读写文件（可能是文件不存在或文件名存在问题）
3. Standard IO Stream 开启失败
4. 编解码失败
5. 无法创建输出文件夹（可能是输出文件夹是一个已存在的文件）
*/

if (args is ["debug", string]) {
	var lower = args[1].ToLower(new CultureInfo("en-US", false));
	if (lower == "enable") {
		Console.WriteLine("$env:Base16384_Net_Debug = 1");
		Console.WriteLine("export Base16384_Net_Debug=1");
		Console.WriteLine("set Base16384_Net_Debug=1");
		return 0;
	}
	if (lower == "disable") {
		Console.WriteLine("$env:Base16384_Net_Debug = $null");
		Console.WriteLine("unset Base16384_Net_Debug");
		Console.WriteLine("set Base16384_Net_Debug=");
		return 0;
	}

	Console.WriteLine("Usage: Base16384.Net.exe <\"e\" | \"d\"> <source | \"-\"> [out | \"-\"]");
	return 1;
}


if (args.Length is not 2 and not 3 || args is not ["e", ..] and not ["d", ..]) {
	Console.WriteLine("Usage: Base16384.Net.exe <\"e\" | \"d\"> <source | \"-\"> [out | \"-\"]");
	return 1;
}

bool readFromStdin = args[1] == "-",
	isDefaultOutput = args.Length == 2,
	writeToStdout = (readFromStdin && isDefaultOutput) || (args is [string, string, "-"]), // `x -` or `x x -`
	isEncodeMode = args[0] == "e"; // Encode mode or decode mode


if (readFromStdin) { // Read from stdin
	try {
		using var stdin = Console.OpenStandardInput();

		if (writeToStdout) { // Write to stdout
			return Helpers.WriteToStdOut(isEncodeMode ? Base16384.EncodeToStream : Base16384.DecodeToStream, stdin);
		}

		// Write to file

		FileInfo output = new(args[2]);

		return output.Exists
			? Helpers.PrintErrorMessage("The output file already exists.", null, 2)
			: Helpers.WriteToFile(isEncodeMode ? Base16384.EncodeToNewFile : Base16384.DecodeToNewFile,
			 stdin, output, "<stdin>");

	} catch (Exception e) {

		// If write to stdout, do not print message
		return Helpers.PrintException(writeToStdout ? null : "Can not open stdin.", e, 3);

	}
}

// Read from file

if (writeToStdout) { // Write to stdout

	if (args[1].Contains(',')) {
		return Helpers.PrintErrorMessage(null, "Source file cannot be a list when writing to stdout.", 2);
	}

	if (!File.Exists(args[1])) {
		return Helpers.PrintErrorMessage(null, "Source file not found.", 2);
	}

	return Helpers.WriteToStdOut(isEncodeMode ? Base16384.EncodeFromFileToStream : Base16384.DecodeFromFileToStream, new FileInfo(args[1]));
}


// Write to file

var codedString = $"{(isEncodeMode ? "en" : "de")}coded";

if (!isDefaultOutput && args[2].Contains(',')) {
	return Helpers.PrintErrorMessage("Output file or directory cannot be a list.", null, 2);
}

// Encode or decode multiple files
if (args[1].Contains(',')) {
	if (args[1].StartsWith(',') || args[1].EndsWith(',')) {
		return Helpers.PrintErrorMessage("Invalid file list.", null, 2);
	}

	DirectoryInfo outputDirectoryInfo = new(isDefaultOutput ? codedString : args[2]);

	if (!outputDirectoryInfo.Exists) { // Try to create output directory
		if (File.Exists(outputDirectoryInfo.FullName)) {
			return Helpers.PrintErrorMessage("Output directory is an existing file.", null, 5);
		}

		try {
			outputDirectoryInfo.Create();
		} catch (Exception e) {
			return Helpers.PrintException("Failed to create output directory.", e, 5);
		}
	}

	FileInfo? input = null, output = null;

	// foreach files in the list
	foreach (var file in args[1].Split(',')) {
		input = new(file);
		output = new(Path.Combine(outputDirectoryInfo.FullName, input.Name));

		if (output.Exists) {
			Console.WriteLine($"{input.Name} -> {output.Name} ... The output file in the directory \"{outputDirectoryInfo.Name}\" already exists.");
			continue;
		}

		if (!input.Exists) {
			Console.WriteLine($"{input.Name} -> {output.Name} ... Source file not found.");
			continue;
		}

		Helpers.Execute(isEncodeMode ? Base16384.EncodeFromFileToNewFile : Base16384.DecodeFromFileToNewFile,
						input, output, input.Name, output.Name);
	}

	return 0;

}

// File not found or Directory exists
if (!File.Exists(args[1])) {

	if (Directory.Exists(args[1])) { // Encode or decode all files in the directory

		DirectoryInfo inputDirectoryInfo = new(args[1]),
			outputDirectoryInfo = new(isDefaultOutput ? codedString : args[2]);

		if (!outputDirectoryInfo.Exists) { // Try to create output directory
			if (File.Exists(outputDirectoryInfo.FullName)) {
				return Helpers.PrintErrorMessage("Output directory is an existing file.", null, 5);
			}

			try {
				outputDirectoryInfo.Create();
			} catch (Exception e) {
				return Helpers.PrintException("Failed to create output directory.", e, 5);
			}
		}

		FileInfo? output = null;
		DirectoryInfo? outputParentDirectoryInfo = null;
		string? relativePath = null, relativeDirectory = null;

		// foreach files in the list
		foreach (var input in inputDirectoryInfo.EnumerateFiles("*", SearchOption.AllDirectories)) {
			relativePath = Path.GetRelativePath(inputDirectoryInfo.FullName, input.FullName);
			output = new(Path.Combine(outputDirectoryInfo.FullName, relativePath));
			relativeDirectory = Path.GetDirectoryName(relativePath)!;
			relativeDirectory = string.IsNullOrWhiteSpace(relativeDirectory) ? "." : relativeDirectory;

			outputParentDirectoryInfo = output.Directory!;

			if (!outputParentDirectoryInfo.Exists) {
				if (File.Exists(outputParentDirectoryInfo.FullName)) {
					Console.WriteLine($"{input.Name} -> {output.Name} ... The output directory \"{relativeDirectory}\" is an existing file.");
				}

				try {
					outputParentDirectoryInfo.Create();
				} catch (Exception e) {
					_ = Helpers.PrintException($"{input.Name} -> {output.Name} ... Failed to create output directory \"{outputParentDirectoryInfo.FullName}\".", e, 5);
					continue;
				}
			}

			if (output.Exists) {
				Console.WriteLine($"{input.Name} -> {output.Name} ... The output file in the directory \"{relativeDirectory}\" already exists.");
				continue;
			}

			if (!input.Exists) {
				Console.WriteLine($"{input.Name} -> {output.Name} ... Source file not found.");
				continue;
			}

			Helpers.Execute(isEncodeMode ? Base16384.EncodeFromFileToNewFile : Base16384.DecodeFromFileToNewFile,
							input, output, input.Name, output.Name);
		}

		return 0;
	}

	return Helpers.PrintErrorMessage("Source file not found.", null, 2);
}

// Encode or decode single file
FileInfo inputFileInfo = new(args[1]),
		outputFileInfo = new(isDefaultOutput ? $"{args[1]}.{codedString}" : args[2]);

if (outputFileInfo.Exists) {
	return Helpers.PrintErrorMessage($"{inputFileInfo.Name} -> {outputFileInfo.Name} ... The output file already exists.", null, 2);
}

if (!inputFileInfo.Exists) {
	return Helpers.PrintErrorMessage(null, $"{inputFileInfo.Name} -> Source file not found.", 2);
}

Helpers.Execute(isEncodeMode ? Base16384.EncodeFromFileToNewFile : Base16384.DecodeFromFileToNewFile,
	inputFileInfo, outputFileInfo, inputFileInfo.Name, outputFileInfo.Name);

return 0;