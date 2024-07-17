using System.Collections.ObjectModel;
using System.Globalization;

namespace Customization;

public sealed record MyResult : IReusable
{
    public DateTime Timestamp { get; init; }
    public double? Sma { get; init; }

    double IReusable.Value
        => Sma.Null2NaN();
}

public static class CustomIndicator
{
    // SERIES, from CHAIN
    public static IReadOnlyList<MyResult> GetIndicator<T>(
        this IEnumerable<T> source,
        int lookbackPeriods)
        where T : IReusable
        => source
            .ToSortedCollection()
            .CalcIndicator(lookbackPeriods);

    private static List<MyResult> CalcIndicator<T>(
        this Collection<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }

        // initialize
        int length = source.Count;
        List<MyResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double? sma;

            if (i >= lookbackPeriods - 1)
            {
                double sum = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                sma = (sum / lookbackPeriods).NaN2Null();
            }
            else
            {
                sma = null;
            }

            results.Add(new() {
                Timestamp = s.Timestamp,
                Sma = sma
            });
        }

        return results;
    }
}

[TestClass]
public class CustomIndicatorTests
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);

    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> badQuotes = Data.GetBad();
    private static readonly IReadOnlyList<Quote> noquotes = [];
    private static readonly IReadOnlyList<Quote> onequote = Data.GetDefault(1);

    [TestMethod]
    public void Standard()
    {
        IReadOnlyList<MyResult> results = quotes
            .GetIndicator(20);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        Assert.IsNull(results[18].Sma);
        Assert.AreEqual(214.5250, Math.Round(results[19].Sma.Value, 4));
        Assert.AreEqual(215.0310, Math.Round(results[24].Sma.Value, 4));
        Assert.AreEqual(234.9350, Math.Round(results[149].Sma.Value, 4));
        Assert.AreEqual(255.5500, Math.Round(results[249].Sma.Value, 4));
        Assert.AreEqual(251.8600, Math.Round(results[501].Sma.Value, 4));
    }

    [TestMethod]
    public void CandlePartOpen()
    {
        IReadOnlyList<MyResult> results = quotes
            .Use(CandlePart.Open)
            .GetIndicator(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        Assert.IsNull(results[18].Sma);
        Assert.AreEqual(214.3795, Math.Round(results[19].Sma.Value, 4));
        Assert.AreEqual(214.9535, Math.Round(results[24].Sma.Value, 4));
        Assert.AreEqual(234.8280, Math.Round(results[149].Sma.Value, 4));
        Assert.AreEqual(255.6915, Math.Round(results[249].Sma.Value, 4));
        Assert.AreEqual(253.1725, Math.Round(results[501].Sma.Value, 4));
    }

    [TestMethod]
    public void CandlePartVolume()
    {
        IReadOnlyList<MyResult> results = quotes
            .Use(CandlePart.Volume)
            .GetIndicator(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        MyResult r24 = results[24];
        Assert.AreEqual(77293768.2, r24.Sma);

        MyResult r290 = results[290];
        Assert.AreEqual(157958070.8, r290.Sma);

        MyResult r501 = results[501];
        Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture), r501.Timestamp);
        Assert.AreEqual(163695200, r501.Sma);
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<EmaResult> results = quotes
            .GetIndicator(10)
            .GetEma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public void QuoteToSortedList()
    {
        IEnumerable<Quote> mismatch = Data.GetMismatch();

        Collection<Quote> h = mismatch.ToSortedCollection();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(firstDate, h[0].Timestamp);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Timestamp);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[50].Timestamp);
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<MyResult> r = Data.GetBtcUsdNan()
            .GetIndicator(50);

        Assert.AreEqual(0, r.Count(x => x.Sma is double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        IReadOnlyList<MyResult> r = badQuotes
            .GetIndicator(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Sma is double.NaN));
    }

    [TestMethod]
    public void NoQuotesExist()
    {
        IReadOnlyList<MyResult> r0 = noquotes
            .GetIndicator(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<MyResult> r1 = onequote
            .GetIndicator(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<MyResult> results = quotes
            .GetIndicator(20)
            .RemoveWarmupPeriods(19);

        Assert.AreEqual(502 - 19, results.Count);
        Assert.AreEqual(251.8600, Math.Round(results[^1].Sma.Value, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetIndicator(0));
}
