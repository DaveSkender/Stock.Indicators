using System.Globalization;

namespace Tests.Common;

// TEST QUOTE IMPORTER
internal static class Importer
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);

    // importer / parser
    internal static Quote QuoteFromCsv(string csvLine)
    {
        if (string.IsNullOrEmpty(csvLine))
        {
            return new();
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
}
