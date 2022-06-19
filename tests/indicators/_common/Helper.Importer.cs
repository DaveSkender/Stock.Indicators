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

// IMPORT TEST DATA
internal class TestData
{
    // DEFAULT: S&P 500 ~2 years of daily data
    internal static IEnumerable<Quote> GetDefault(int days = 502)
        => File.ReadAllLines("_common/data/default.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // RANDOM: gaussian brownaian motion
    internal static IEnumerable<Quote> GetRandom(int days = 502)
        => new RandomGbm(bars: days);

    // BAD DATA
    internal static IEnumerable<Quote> GetBad(int days = 502)
        => File.ReadAllLines("_common/data/bad.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // TOO BIG DATA
    internal static IEnumerable<Quote> GetTooBig(int days = 1246)
        => File.ReadAllLines("_common/data/toobig.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // MAX SIZE DATA
    internal static IEnumerable<Quote> GetMax(int days = 502)
        => File.ReadAllLines("_common/data/toobig.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // BITCOIN DATA
    internal static IEnumerable<Quote> GetBitcoin(int days = 1246)
        => File.ReadAllLines("_common/data/bitcoin.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IEnumerable<Quote> GetCompare(int days = 502)
        => File.ReadAllLines("_common/data/compare.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // INTRADAY DATA
    internal static IEnumerable<Quote> GetIntraday(int days = 1564)
        => File.ReadAllLines("_common/data/intraday.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // LONGISH DATA ~20 years of S&P 500 daily data
    internal static IEnumerable<Quote> GetLongish(int days = 5285)
        => File.ReadAllLines("_common/data/longish.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // LONGEST DATA ~62 years of S&P 500 daily data
    internal static IEnumerable<Quote> GetLongest()
        => File.ReadAllLines("_common/data/longest.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .ToList();

    // PENNY DATA
    internal static IEnumerable<Quote> GetPenny()
        => File.ReadAllLines("_common/data/penny.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .ToList();

    // MISMATCH DATA is in incorrect sequence
    internal static IEnumerable<Quote> GetMismatch()
        => File.ReadAllLines("_common/data/mismatch.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .ToList();

    // SPX, 30 years, daily
    internal static IEnumerable<Quote> GetSpx(int days = 8111)
        => File.ReadAllLines("_common/data/spx.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // MSFT, 30 years, daily
    internal static IEnumerable<Quote> GetMsft(int days = 8111)
        => File.ReadAllLines("_common/data/msft.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();
}
