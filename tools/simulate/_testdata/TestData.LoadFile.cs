using Skender.Stock.Indicators;

namespace Test.Data;

// TEST QUOTE GETTERs

internal static class Data
{
    // sorted by filename

    // DEFAULT: SnP 500 ~2 years of daily data
    internal static IReadOnlyList<Quote> GetDefault(int days = 502)
        => File.ReadAllLines("_testdata/quotes/default.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderBy(static x => x.Timestamp)
            .Take(days)
            .ToList();

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IReadOnlyList<Quote> GetCompare(int days = 502)
        => File.ReadAllLines("_testdata/quotes/compare.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderBy(static x => x.Timestamp)
            .Take(days)
            .ToList();

    // INTRADAY DATA
    internal static IReadOnlyList<Quote> GetIntraday(int days = 1564)
        => File.ReadAllLines("_testdata/quotes/intraday.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderBy(static x => x.Timestamp)
            .Take(days)
            .ToList();

    // LONGEST DATA ~62 years of SnP 500 daily data
    internal static IReadOnlyList<Quote> GetLongest()
        => File.ReadAllLines("_testdata/quotes/longest.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderBy(static x => x.Timestamp)
            .ToList();

    // LONGISH DATA ~20 years of SnP 500 daily data
    internal static IReadOnlyList<Quote> GetLongish(int days = 5285)
        => File.ReadAllLines("_testdata/quotes/longish.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderBy(static x => x.Timestamp)
            .Take(days)
            .ToList();
}
