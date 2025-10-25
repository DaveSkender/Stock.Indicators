using System.Globalization;

namespace Tests.Data;

internal static partial class Utilities
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);

    /// <summary>
    /// importer / parser
    /// </summary>
    /// <param name="csvLine"></param>
    /// <returns></returns>
    internal static Quote QuoteFromCsv(string csvLine)
    {
        if (string.IsNullOrEmpty(csvLine))
        {
            throw new InvalidDataException("CSV line was empty");
        }

        string[] csv = csvLine.Split(',');

        Quote quote = new(
            Timestamp: DateTime.TryParse(csv[0], EnglishCulture, out DateTime d) ? d : default,
            Open: csv[1].ToDecimalDefault(),
            High: csv[2].ToDecimalDefault(),
            Low: csv[3].ToDecimalDefault(),
            Close: csv[4].ToDecimalDefault(),
            Volume: csv[5].ToDecimalDefault()
        );

        return quote;
    }

    internal static decimal ToDecimal(this string value)
        => decimal.TryParse(value, out decimal d) ? d
            : throw new NotFiniteNumberException(
                $"Cannot convert `{value}`,  it is not a number.");

    internal static decimal ToDecimalDefault(this string value)
        => decimal.TryParse(value, out decimal d) ? d : default;

    internal static decimal? ToDecimalNull(this string value)
        => decimal.TryParse(value, out decimal d) ? d : null;

    internal static double? ToDoubleNull(this string value)
        => double.TryParse(value, out double d) ? d : null;

    internal static string WithDefaultLineEndings(this string input)
        => input
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\n", Environment.NewLine, StringComparison.Ordinal);
}
