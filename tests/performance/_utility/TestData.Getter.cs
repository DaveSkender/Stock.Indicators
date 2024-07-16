namespace Test.Data;

// TEST QUOTE GETTERs

internal static class Data
{
    // sorted by filename

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IReadOnlyList<Quote> GetCompare(int days = 502)
        => File.ReadAllLines("_utility/data/compare.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // DEFAULT: S&P 500 ~2 years of daily data
    internal static IReadOnlyList<Quote> GetDefault(int days = 502)
        => File.ReadAllLines("_utility/data/default.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // INTRADAY DATA
    internal static IReadOnlyList<Quote> GetIntraday(int days = 1564)
        => File.ReadAllLines("_utility/data/intraday.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // LONGEST DATA ~62 years of S&P 500 daily data
    internal static IReadOnlyList<Quote> GetLongest()
        => File.ReadAllLines("_utility/data/longest.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .ToList();

    // LONGISH DATA ~20 years of S&P 500 daily data
    internal static IReadOnlyList<Quote> GetLongish(int days = 5285)
        => File.ReadAllLines("_utility/data/longish.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();
}
