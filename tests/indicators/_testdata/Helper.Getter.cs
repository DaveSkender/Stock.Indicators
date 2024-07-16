namespace Utilities;

// IMPORT TEST DATA
internal static class TestData
{
    // sorted by filename

    // RANDOM: gaussian brownaian motion
    internal static IEnumerable<Quote> GetRandom(int days = 502)
        => new RandomGbm(bars: days);

    // BAD DATA
    internal static IEnumerable<Quote> GetBad(int days = 502)
        => File.ReadAllLines("_testdata/data/bad.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // BITCOIN DATA
    internal static IEnumerable<Quote> GetBitcoin(int days = 1246)
        => File.ReadAllLines("_testdata/data/bitcoin.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // BTCUSD, 69288 records, 15-minute bars
    internal static IEnumerable<Quote> GetBtcUsdNan(int bars = 69288)
        => File.ReadAllLines("_testdata/data/btcusd15x69k.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(bars)
            .ToList();

    // DEFAULT: S&P 500 ~2 years of daily data
    internal static IEnumerable<Quote> GetDefault(int days = 502)
        => File.ReadAllLines("_testdata/data/default.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IEnumerable<Quote> GetCompare(int days = 502)
        => File.ReadAllLines("_testdata/data/compare.csv")
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

    // MISMATCH DATA is in incorrect sequence
    internal static IEnumerable<Quote> GetMismatch()
        => File.ReadAllLines("_testdata/data/mismatch.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .ToList();

    // MSFT, 30 years, daily
    internal static IEnumerable<Quote> GetMsft(int days = 8111)
        => File.ReadAllLines("_testdata/data/msft.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // PENNY DATA
    internal static IEnumerable<Quote> GetPenny()
        => File.ReadAllLines("_testdata/data/penny.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .ToList();

    // SPX, 30 years, daily
    internal static IEnumerable<Quote> GetSpx(int days = 8111)
        => File.ReadAllLines("_testdata/data/spx.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // TOO BIG DATA
    internal static IEnumerable<Quote> GetTooBig(int days = 1246)
        => File.ReadAllLines("_testdata/data/toobig.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // MAX SIZE DATA
    internal static IEnumerable<Quote> GetMax(int days = 502)
        => File.ReadAllLines("_testdata/data/toobig.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();

    // ZEROS (200)
    internal static IEnumerable<Quote> GetZeros(int days = 200)
        => File.ReadAllLines("_testdata/data/zeros.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToList();
}
