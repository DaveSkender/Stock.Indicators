namespace Test.Utilities;

internal static class StringUtilities
{
    internal static string WithDefaultLineEndings(this string input)
        => input
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\n", Environment.NewLine, StringComparison.Ordinal);
}
