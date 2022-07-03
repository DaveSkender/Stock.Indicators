using Skender.Stock.Indicators;

namespace Internal.Tests;

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

    // BTCUSD, 69288 records, 15-minute bars
    internal static IEnumerable<Quote> GetBtcUsdNan(int bars = 69288)
        => File.ReadAllLines("_common/data/btcusd15x69k.csv")
            .Skip(1)
            .Select(v => Importer.QuoteFromCsv(v))
            .OrderByDescending(x => x.Date)
            .Take(bars)
            .ToList();

    // TUPLE with NaNs
    internal static IEnumerable<(DateTime, double)> GetTupleNaN()
    {
        List<(DateTime, double)> tpList = new(200);
        double timeFactor = 10000000d;

        DateTime date = DateTime.UtcNow;

        // sequential
        for (int i = 0; i < 25; i++)
        {
            date = date.AddDays(1);
            double value = date.ToFileTime() / timeFactor;

            tpList.Add(new(date, value));
        }

        // sequential negative
        for (int i = 0; i < 25; i++)
        {
            date = date.AddDays(1);
            double value = -date.ToFileTime() / timeFactor;

            tpList.Add(new(date, value));
        }

        // sequential 0
        for (int i = 0; i < 25; i++)
        {
            date = date.AddDays(1);
            double value = 0;

            tpList.Add(new(date, value));
        }

        // sequential -10
        for (int i = 0; i < 25; i++)
        {
            date = date.AddDays(1);
            double value = -10;

            tpList.Add(new(date, value));
        }

        // sequential 10
        for (int i = 0; i < 25; i++)
        {
            date = date.AddDays(1);
            double value = 10;

            tpList.Add(new(date, value));
        }

        // some NaNs
        for (int i = 0; i < 25; i++)
        {
            date = date.AddDays(1);
            double value = double.NaN;

            tpList.Add(new(date, value));
        }

        // more sequential
        for (int i = 0; i < 50; i++)
        {
            date = date.AddDays(1);
            double value = date.ToFileTime() / timeFactor;

            tpList.Add(new(date, value));
        }

        return tpList;
    }
}
