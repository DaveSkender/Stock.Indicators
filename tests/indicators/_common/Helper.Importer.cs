using System.Globalization;
using Skender.Stock.Indicators;

namespace Internal.Tests;

// TEST QUOTE IMPORTER
internal static class Importer
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);

    // importer / parser
    internal static Quote QuoteFromCsv(string csvLine)
    {
        if (string.IsNullOrEmpty(csvLine))
        {
            return new Quote();
        }

        string[] values = csvLine.Split(',');
        Quote quote = new();

        HandleOHLCV(quote, "D", values[0]);
        HandleOHLCV(quote, "O", values[1]);
        HandleOHLCV(quote, "H", values[2]);
        HandleOHLCV(quote, "L", values[3]);
        HandleOHLCV(quote, "C", values[4]);
        HandleOHLCV(quote, "V", values[5]);

        return quote;
    }

    internal static decimal ToDecimal(this string value)
        => decimal.TryParse(value, out decimal d) ? d : d;

    internal static decimal? ToDecimalNull(this string value)
        => decimal.TryParse(value, out decimal d) ? d : null;

    internal static double? ToDoubleNull(this string value)
        => double.TryParse(value, out double d) ? d : null;

    private static void HandleOHLCV(Quote quote, string position, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        switch (position)
        {
            case "D":
                quote.Date = Convert.ToDateTime(value, EnglishCulture);
                break;
            case "O":
                quote.Open = Convert.ToDecimal(value, EnglishCulture);
                break;
            case "H":
                quote.High = Convert.ToDecimal(value, EnglishCulture);
                break;
            case "L":
                quote.Low = Convert.ToDecimal(value, EnglishCulture);
                break;
            case "C":
                quote.Close = Convert.ToDecimal(value, EnglishCulture);
                break;
            case "V":
                quote.Volume = Convert.ToDecimal(value, EnglishCulture);
                break;
            default:
                break;
        }
    }
}
