// 新的解析方式：https://github.com/execute233/Base16384.Net
/*
错误代码：
1. 无法解析命令行参数
2. 无法读取文件（可能是文件不存在）
3. Standard IO Stream 开启失败
4. 编解码失败
*/

if (args.Length is not 2 and not 3 || args is not ["e", ..] and not ["d", ..]) {
	Console.WriteLine("Usage: Base16384.Net.exe <\"e\" | \"d\"> <source | \"-\"> [out | \"-\"]");
	return 1;
}

if (args[0] == "e") {
	// Encode mode
	if (args[1] == "-") {
		// Read from stdin
		try {
			using var stdin = Console.OpenStandardInput();
			if (args.Length == 2 || args[2] == "-") {
				// Write to stdout
				try {
					using var stdout = Console.OpenStandardOutput();
					try {
						Base16384.EncodeToStream(stdin, stdout);
					} catch {
						return 4;
					}
				} catch {
					return 3;
				}
			} else {
				// Write to file
				FileInfo info = new(args[2]);
				Console.Write($"<stdin> -> {info.Name} ... ");
				try {
					Base16384.EncodeToNewFile(stdin, info);
				} catch {
					Console.WriteLine("Failed.");
					return 4;
				}
				Console.WriteLine("Done.");
			}
		} catch {
			if (args.Length != 2 && args[2] != "-") {
				Console.WriteLine("Can not open stdin.");
			}
			return 3;
		}
	} else {
		// Read from file
		if (args is [.., "-"]) {
			// Write to stdout
			if (!File.Exists(args[1])) {
				return 2;
			}
			try {
				using var stdout = Console.OpenStandardOutput();
				try {
					Base16384.EncodeFromFileToStream(new(args[1]), stdout);
				} catch {
					return 4;
				}
			} catch {
				return 3;
			}
		} else if (args.Length == 2) {
			// Write to .decoded file
			if (!File.Exists(args[1])) {
				if (Directory.Exists(args[1])) {
					// 此处应该遍历文件夹
					return 0;
				}
				Console.WriteLine("Source file not found.");
				return 2;
			}
			FileInfo info = new(args[1]);
			Console.Write($"{info.Name} -> {info.Name}.encoded ... ");
			try {
				Base16384.EncodeFromFileToNewFile(info, new($"{args[1]}.encoded"));
			} catch {
				Console.WriteLine("Failed.");
				return 4;
			}
			Console.WriteLine("Done.");
		} else {
			// Write to file
			if (!File.Exists(args[1])) {
				if (Directory.Exists(args[1])) {
					// 此处应该遍历文件夹
					return 0;
				}
				Console.WriteLine("Source file not found.");
				return 2;
			}
			Console.Write($"{args[1]} -> {args[2]} ... ");
			try {
				Base16384.EncodeFromFileToNewFile(new(args[1]), new(args[2]));
			} catch {
				Console.WriteLine("Failed.");
				return 4;
			}
			Console.WriteLine("Done.");
		}
	}
} else {
	// Decode mode
	if (args[1] == "-") {
		// Read from stdin
		try {
			using var stdin = Console.OpenStandardInput();
			if (args.Length == 2 || args[2] == "-") {
				// Write to stdout
				try {
					using var stdout = Console.OpenStandardOutput();
					try {
						Base16384.DecodeToStream(stdin, stdout);
					} catch {
						return 4;
					}
				} catch {
					return 3;
				}
			} else {
				// Write to file
				FileInfo info = new(args[2]);
				Console.Write($"<stdin> -> {info.Name} ... ");
				try {
					Base16384.DecodeToNewFile(stdin, info);
				} catch {
					Console.WriteLine("Failed.");
					return 4;
				}
				Console.WriteLine("Done.");
			}
		} catch {
			if (args.Length != 2 && args[2] != "-") {
				Console.WriteLine("Can not open stdin.");
			}
			return 3;
		}
	} else { // 异常处理和用户提示还没完成
			 // Read from file
		if (args is [.., "-"]) {
			// Write to stdout
			if (!File.Exists(args[1])) {
				Console.WriteLine("Source file not found.");
				return 2;
			}
			using var stdout = Console.OpenStandardOutput();
			Base16384.DecodeFromFileToStream(new(args[1]), stdout);
		} else if (args.Length == 2) {
			// Write to .decoded file
			if (!File.Exists(args[1])) {
				if (Directory.Exists(args[1])) {
					// 此处应该遍历文件夹
					return 0;
				}
				Console.WriteLine("Source file not found.");
				return 2;
			}
			Base16384.DecodeFromFileToNewFile(new(args[1]), new($"{args[1]}.decoded"));
		} else {
			// Write to file
			if (!File.Exists(args[1])) {
				if (Directory.Exists(args[1])) {
					// 此处应该遍历文件夹
					return 0;
				}
				Console.WriteLine("Source file not found.");
				return 2;
			}
			Base16384.DecodeFromFileToNewFile(new(args[1]), new(args[2]));
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