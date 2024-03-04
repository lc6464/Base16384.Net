using System.Diagnostics.CodeAnalysis;

namespace ConsoleHelpers;

public static class Helpers {
	public static readonly string? debugEnv = Environment.GetEnvironmentVariable("Base16384_Net_Debug");
	public static readonly bool DEBUG = debugEnv is not null && debugEnv.ToLower(new CultureInfo("en-US", false)) is not "false" and not "0";


	public static int PrintErrorMessage(string? message, string? debugMessage = null, int exitCode = 0) {
		if (message is not null) {
			Console.WriteLine(message);
		}
		if (DEBUG && !string.IsNullOrWhiteSpace(debugMessage)) {
			Console.Error.WriteLine(debugMessage);
		}
		return exitCode;
	}

	public static int PrintException(string? message, [NotNull] Exception e, int exitCode = 0) =>
		PrintErrorMessage(message, e.ToString(), exitCode);


	public static int Execute<TIn, TOut>(Func<TIn, TOut, long> func, TIn input, TOut output, string? inputName, string? outputName, bool isStdOut = false) {
		if (!isStdOut) {
			Console.Write($"{inputName} -> {outputName} ... ");
		}
		try {
			func(input, output);
		} catch (Exception e) {
			return PrintException(isStdOut ? null : "Failed.", e, 4);
		}
		if (!isStdOut) {
			Console.WriteLine("Done.");
		}
		return 0;
	}

	public static int WriteToStdOut<T>(Func<T, Stream, long> func, T input) {
		try {
			using var stdout = Console.OpenStandardOutput();
			return Execute(func, input, stdout, null, null, true);
		} catch (Exception e) {
			return PrintException(null, e, 3);
		}
	}

	public static int WriteToFile<T>(Func<T, FileInfo, long> func, T input, FileInfo output, string inputName) =>
		Execute(func, input, output, inputName, output.Name);
}