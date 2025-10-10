using System.Text.Json;

namespace Tests.Data;

// TEST DATA FILE LOADERS

internal static class Data
{
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    internal static IReadOnlyList<TResult> Results<TResult>(string filename)
        where TResult : ISeries
    {
        // Check if baseline file exists
        string filepath = Path.Combine("_testdata", "results", filename);

        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException("Test data file not found.", filepath);
        }

        // Load and deserialize file
        string json = File.ReadAllText(filepath);
        List<TResult> expectedResults = JsonSerializer.Deserialize<List<TResult>>(json, JsonOptions);

        Assert.IsNotNull(expectedResults, $"Failed to deserialize baseline file: {filepath}");

        return expectedResults;
    }

    // DEFAULT: S&P 500 ~2 years of daily data
    internal static IReadOnlyList<Quote> GetDefault(int days = 502)
        => File.ReadAllLines("_testdata/quotes/default.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // RANDOM: gaussian brownaian motion
    internal static IReadOnlyList<Quote> GetRandom(
        int bars = 502,
        PeriodSize periodSize = PeriodSize.OneMinute,
        bool includeWeekends = true)
        => new RandomGbm(
            bars: bars,
            periodSize: periodSize,
            includeWeekends: includeWeekends);

    // sorted by filename

    // BAD DATA
    internal static IReadOnlyList<Quote> GetBad(int days = 502)
        => File.ReadAllLines("_testdata/quotes/bad.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // BITCOIN DATA
    internal static IReadOnlyList<Quote> GetBitcoin(int days = 1246)
        => File.ReadAllLines("_testdata/quotes/bitcoin.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // BTCUSD, 69288 records, 15-minute bars
    internal static IReadOnlyList<Quote> GetBtcUsdNan(int bars = 69288)
        => File.ReadAllLines("_testdata/quotes/btcusd15x69k.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(bars)
            .ToSortedList();

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IReadOnlyList<Quote> GetCompare(int days = 502)
        => File.ReadAllLines("_testdata/quotes/compare.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // INTRADAY DATA
    internal static IReadOnlyList<Quote> GetIntraday(int days = 1564)
        => File.ReadAllLines("_testdata/quotes/intraday.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // LONGEST DATA ~62 years of S&P 500 daily data (15,821)
    internal static IReadOnlyList<Quote> GetLongest()
        => File.ReadAllLines("_testdata/quotes/longest.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .ToSortedList();

    // LONGISH DATA ~20 years of S&P 500 daily data
    internal static IReadOnlyList<Quote> GetLongish(int days = 5285)
        => File.ReadAllLines("_testdata/quotes/longish.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // MISMATCH DATA is in incorrect sequence
    internal static IReadOnlyList<Quote> GetMismatch()
        => File.ReadAllLines("_testdata/quotes/mismatch.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .ToList();  // not sorted

    // MSFT, 30 years, daily
    internal static IReadOnlyList<Quote> GetMsft(int days = 8111)
        => File.ReadAllLines("_testdata/quotes/msft.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // PENNY DATA
    internal static IReadOnlyList<Quote> GetPenny()
        => File.ReadAllLines("_testdata/quotes/penny.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .ToSortedList();

    // SPX, 30 years, daily
    internal static IReadOnlyList<Quote> GetSpx(int days = 8111)
        => File.ReadAllLines("_testdata/quotes/spx.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // TOO BIG DATA
    internal static IReadOnlyList<Quote> GetTooBig(int days = 1246)
        => File.ReadAllLines("_testdata/quotes/toobig.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // MAX SIZE DATA
    internal static IReadOnlyList<Quote> GetMax(int days = 502)
        => File.ReadAllLines("_testdata/quotes/toobig.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // ZEROS (200)
    internal static IReadOnlyList<Quote> GetZeros(int days = 200)
        => File.ReadAllLines("_testdata/quotes/zeros.csv")
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .Take(days)
            .ToSortedList();
}
