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