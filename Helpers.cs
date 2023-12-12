namespace ConsoleHelpers;

public static class Helpers {
	public static int WriteToStdOut<T>(Func<T, Stream, long> func, T input, Stream? stdout = null) {
		try {
			stdout ??= Console.OpenStandardOutput();
			try {
				func(input, stdout);
			} catch {
				return 4;
			} finally {
				stdout.Dispose();
			}
		} catch {
			return 3;
		}
		return 0;
	}

	public static int WriteToFile<T>(Func<T, FileInfo, long> func, T input, string inputName, FileInfo output) {
		Console.Write($"{inputName} -> {output.Name} ... ");
		try {
			func(input, output);
		} catch {
			Console.WriteLine("Failed.");
			return 4;
		}
		Console.WriteLine("Done.");
		return 0;
	}
}