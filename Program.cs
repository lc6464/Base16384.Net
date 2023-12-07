// 新的解析方式：https://github.com/execute233/Base16384.Net

if (args.Length is not 2 and not 3 || args is not ["e", ..] and not ["d", ..]) {
	Console.WriteLine("Usage: Base16384.Net.exe <\"e\" | \"d\"> <source | \"-\"> [out | \"-\"]");
	return 1;
}

if (args[0] == "e") {
	// Encode mode
	if (args[1] == "-") {
		// Read from stdin
		using var stdin = Console.OpenStandardInput();
		if (args.Length == 2 || args[2] == "-") {
			// Write to stdout
			using var stdout = Console.OpenStandardOutput();
			Base16384.EncodeToStream(stdin, stdout);
		} else {
			// Write to file
			Base16384.EncodeToNewFile(stdin, new(args[2]));
		}
	} else {
		// Read from file
		if (args is [.., "-"]) {
			// Write to stdout
			using var stdout = Console.OpenStandardOutput();
			Base16384.EncodeFromFileToStream(new(args[1]), stdout);
		} else if (args.Length == 2) {
			// Write to .decoded file
			Base16384.EncodeFromFileToNewFile(new(args[1]), new($"{args[1]}.encoded"));
		} else {
			// Write to file
			Base16384.EncodeFromFileToNewFile(new(args[1]), new(args[2]));
		}
	}
}


return 0;


/*
if (args.Length == 1) {
	if (args[0] == "e") {
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

			Base16384.EncodeFromFileToNewFile(file, encodedFileInfo);

			Console.WriteLine($"{file.Name} - Finished.");
		}

		Console.WriteLine("\nAll done.");
	} else if (args[0] == "d") {
		DirectoryInfo directoryInfo = new("Encoded");
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

			Base16384.DecodeFromFileToNewFile(file, decodedFileInfo);

			Console.WriteLine($"{file.Name} - Finished.");
		}

		Console.WriteLine("\nAll done.");
	} else {
		Console.WriteLine("Unknown argument.");
		return 4;
	}
	return 0;
}


Console.WriteLine("Unknown argument.");

return 4;

*/