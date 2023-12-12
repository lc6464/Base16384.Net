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
}