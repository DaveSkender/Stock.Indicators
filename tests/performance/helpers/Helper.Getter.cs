namespace Tests.Common;

// IMPORT TEST DATA
internal static class TestData
{
    // DEFAULT: S&P 500 ~2 years of daily data
    internal static IEnumerable<Quote> GetDefault(int days = 502)
        => File.ReadAllLines("helpers/data/default.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IEnumerable<Quote> GetCompare(int days = 502)
        => File.ReadAllLines("helpers/data/compare.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // INTRADAY DATA
    internal static IEnumerable<Quote> GetIntraday(int days = 1564)
        => File.ReadAllLines("helpers/data/intraday.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();

    // LONGISH DATA ~20 years of S&P 500 daily data
    internal static IEnumerable<Quote> GetLongish(int days = 5285)
        => File.ReadAllLines("helpers/data/longish.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Date)
            .Take(days)
            .ToList();
}
