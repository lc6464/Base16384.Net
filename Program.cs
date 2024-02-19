// 新的解析方式：https://github.com/execute233/Base16384.Net
/*
错误代码：
1. 无法解析命令行参数
2. 无法读取文件（可能是文件不存在）
3. Standard IO Stream 开启失败
4. 编解码失败
5. 无法创建输出文件夹（可能是输出文件夹是一个已存在的文件）
*/


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

		return Helpers.WriteToFile(isEncodeMode ? Base16384.EncodeToNewFile : Base16384.DecodeToNewFile,
			 stdin, new(args[2]), "<stdin>");

	} catch (Exception e) {

		// If write to stdout, do not print message
		return Helpers.PrintException(writeToStdout ? null : "Can not open stdin.", e, 3);

	}
}

// Read from file

if (writeToStdout) { // Write to stdout

	return File.Exists(args[1])
		? Helpers.WriteToStdOut(isEncodeMode ? Base16384.EncodeFromFileToStream : Base16384.DecodeFromFileToStream,
			new FileInfo(args[1]))
		: Helpers.PrintErrorMessage(null, "Source file not found.", 2);

}

// Write to file

// File not found or Directory exists
if (!File.Exists(args[1])) {

	if (Directory.Exists(args[1])) { // Encode all files in the directory

		var output = isDefaultOutput ? $"{(isEncodeMode ? "en" : "de")}coded" : args[2];
		DirectoryInfo inputDirectoryInfo = new(args[1]),
			outputDirectoryInfo = new(output);

		if (!outputDirectoryInfo.Exists) { // Try to create output directory
			if (File.Exists(output)) {
				return Helpers.PrintErrorMessage("Output directory is an existing file.", null, 5);
			}

			try {
				outputDirectoryInfo.Create();
			} catch (Exception e) {
				return Helpers.PrintException("Failed to create output directory.", e, 5);
			}
		}

		// 此处应该遍历文件夹

		return 0;
	}

	return Helpers.PrintErrorMessage("Source file not found.", null, 2);
}

// Encode single file

FileInfo inputFileInfo = new(args[1]),
		outputFileInfo = new(isDefaultOutput ? $"{args[1]}.{(isEncodeMode ? "en" : "de")}coded" : args[2]);

return Helpers.Execute(isEncodeMode ? Base16384.EncodeFromFileToNewFile : Base16384.DecodeFromFileToNewFile,
	inputFileInfo, outputFileInfo, inputFileInfo.Name, outputFileInfo.Name);