namespace Test.Data;

// TEST QUOTE GETTERs

internal static class Data
{
    // DEFAULT: S&P 500 ~2 years of daily data
    internal static IReadOnlyList<Quote> GetDefault(int days = 502)
        => File.ReadAllLines("_testdata/data/default.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // RANDOM: gaussian brownaian motion
    internal static IReadOnlyList<Quote> GetRandom(int days = 502)
        => new RandomGbm(bars: days);

    // sorted by filename

    // BAD DATA
    internal static IReadOnlyList<Quote> GetBad(int days = 502)
        => File.ReadAllLines("_testdata/data/bad.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // BITCOIN DATA
    internal static IReadOnlyList<Quote> GetBitcoin(int days = 1246)
        => File.ReadAllLines("_testdata/data/bitcoin.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // BTCUSD, 69288 records, 15-minute bars
    internal static IReadOnlyList<Quote> GetBtcUsdNan(int bars = 69288)
        => File.ReadAllLines("_testdata/data/btcusd15x69k.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(bars)
            .ToSortedList();

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IReadOnlyList<Quote> GetCompare(int days = 502)
        => File.ReadAllLines("_testdata/data/compare.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // INTRADAY DATA
    internal static IReadOnlyList<Quote> GetIntraday(int days = 1564)
        => File.ReadAllLines("_testdata/data/intraday.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // LONGEST DATA ~62 years of S&P 500 daily data
    internal static IReadOnlyList<Quote> GetLongest()
        => File.ReadAllLines("_testdata/data/longest.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .ToSortedList();

    // LONGISH DATA ~20 years of S&P 500 daily data
    internal static IReadOnlyList<Quote> GetLongish(int days = 5285)
        => File.ReadAllLines("_testdata/data/longish.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // MISMATCH DATA is in incorrect sequence
    internal static IReadOnlyList<Quote> GetMismatch()
        => File.ReadAllLines("_testdata/data/mismatch.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .ToList();  // not sorted

    // MSFT, 30 years, daily
    internal static IReadOnlyList<Quote> GetMsft(int days = 8111)
        => File.ReadAllLines("_testdata/data/msft.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // PENNY DATA
    internal static IReadOnlyList<Quote> GetPenny()
        => File.ReadAllLines("_testdata/data/penny.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .ToSortedList();

    // SPX, 30 years, daily
    internal static IReadOnlyList<Quote> GetSpx(int days = 8111)
        => File.ReadAllLines("_testdata/data/spx.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // TOO BIG DATA
    internal static IReadOnlyList<Quote> GetTooBig(int days = 1246)
        => File.ReadAllLines("_testdata/data/toobig.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // MAX SIZE DATA
    internal static IReadOnlyList<Quote> GetMax(int days = 502)
        => File.ReadAllLines("_testdata/data/toobig.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // ZEROS (200)
    internal static IReadOnlyList<Quote> GetZeros(int days = 200)
        => File.ReadAllLines("_testdata/data/zeros.csv")
            .Skip(1)
            .Select(Imports.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();
}
