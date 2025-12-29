using System.Text.Json;

namespace Test.Data;

// TEST DATA FILE LOADERS

internal static class Data
{
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    internal static IReadOnlyList<TResult> ResultsFromJson<TResult>(string filename)
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
        List<TResult> results = JsonSerializer.Deserialize<List<TResult>>(json, JsonOptions);

        Assert.IsNotNull(results, $"Failed to deserialize baseline file: {filepath}");

        return results;
    }

    internal static IReadOnlyList<Quote> QuotesFromJson(string filename)
    {
        // Check if baseline file exists
        string filepath = Path.Combine("_testdata", "quotes", filename);

        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException("Test data file not found.", filepath);
        }

        // Load and deserialize file
        string json = File.ReadAllText(filepath);
        List<Quote> quotes = JsonSerializer.Deserialize<List<Quote>>(json, JsonOptions);

        Assert.IsNotNull(quotes, $"Failed to deserialize baseline file: {filepath}");

        return quotes;
    }

    internal static IReadOnlyList<Quote> QuotesFromCsv(string filename, int days = int.MaxValue)
        => File.ReadAllLines(Path.Combine("_testdata", "quotes", filename))
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .OrderByDescending(static x => x.Timestamp)
            .Take(days)
            .ToSortedList();

    // DEFAULT: SnP 500 ~2 years of daily data
    internal static IReadOnlyList<Quote> GetDefault(int days = 502)
        => QuotesFromCsv("default.csv", days);

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
        => QuotesFromCsv("bad.csv", days);

    // BITCOIN DATA
    internal static IReadOnlyList<Quote> GetBitcoin(int days = 1246)
        => QuotesFromCsv("bitcoin.csv", days);

    // BTCUSD, 69288 records, 15-minute bars
    internal static IReadOnlyList<Quote> GetBtcUsdNan(int bars = 69288)
        => QuotesFromCsv("btcusd15x69k.csv", bars);

    // COMPARE DATA ~2 years of TSLA data (matches default time)
    internal static IReadOnlyList<Quote> GetCompare(int days = 502)
        => QuotesFromCsv("compare.csv", days);

    // INTRADAY DATA
    internal static IReadOnlyList<Quote> GetIntraday(int days = 1564)
        => QuotesFromCsv("intraday.csv", days);

    // LONGEST DATA ~62 years of SnP 500 daily data (15,821)
    internal static IReadOnlyList<Quote> GetLongest()
        => QuotesFromCsv("longest.csv");

    // LONGISH DATA ~20 years of SnP 500 daily data
    internal static IReadOnlyList<Quote> GetLongish(int days = 5285)
        => QuotesFromCsv("longish.csv", days);

    // MISMATCH DATA is in incorrect sequence
    internal static IReadOnlyList<Quote> GetMismatch()
        => File.ReadAllLines(Path.Combine("_testdata", "quotes", "mismatch.csv"))
            .Skip(1)
            .Select(Utilities.QuoteFromCsv)
            .ToList();  // not sorted

    // MSFT, 30 years, daily
    internal static IReadOnlyList<Quote> GetMsft(int days = 8111)
        => QuotesFromCsv("msft.csv", days);

    // PENNY DATA
    internal static IReadOnlyList<Quote> GetPenny()
        => QuotesFromCsv("penny.csv");

    // SPX, 30 years, daily
    internal static IReadOnlyList<Quote> GetSpx(int days = 8111)
        => QuotesFromCsv("spx.csv", days);

    // TOO BIG DATA
    internal static IReadOnlyList<Quote> GetTooBig(int days = 1246)
        => QuotesFromCsv("toobig.csv", days);

    // MAX SIZE DATA
    internal static IReadOnlyList<Quote> GetMax(int days = 502)
        => QuotesFromCsv("toobig.csv", days);

    // ZEROS (200)
    internal static IReadOnlyList<Quote> GetZeros(int days = 200)
        => QuotesFromCsv("zeros.csv", days);
}
