namespace Utilities;

// IMPORT TEST DATA
internal static class TestData
{
    // sorted by filename

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IEnumerable<Quote> GetCompare(int days = 502)
        => File.ReadAllLines("_testdata/data/compare.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // DEFAULT: S&P 500 ~2 years of daily data
    internal static IEnumerable<Quote> GetDefault(int days = 502)
        => File.ReadAllLines("_testdata/data/default.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // INTRADAY DATA
    internal static IEnumerable<Quote> GetIntraday(int days = 1564)
        => File.ReadAllLines("_testdata/data/intraday.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // LONGEST DATA ~62 years of S&P 500 daily data
    internal static IEnumerable<Quote> GetLongest()
        => File.ReadAllLines("_testdata/data/longest.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .ToList();

    // LONGISH DATA ~20 years of S&P 500 daily data
    internal static IEnumerable<Quote> GetLongish(int days = 5285)
        => File.ReadAllLines("_testdata/data/longish.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();
}
