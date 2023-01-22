using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.CustomIndicators;

public sealed class MyResult : ResultBase, IReusableResult
{
    public MyResult(DateTime date)
    {
        Date = date;
    }

    public double? Sma { get; set; }

    double? IReusableResult.Value => Sma;
}

public static class CustomIndicator
{
    // SERIES, from TQuote
    public static IEnumerable<MyResult> GetIndicator<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToTupleCollection(CandlePart.Close)
            .CalcIndicator(lookbackPeriods);

    // SERIES, from CHAIN
    public static IEnumerable<MyResult> GetIndicator(
        this IEnumerable<IReusableResult> results,
        int lookbackPeriods) => results
            .ToTupleChainable()
            .CalcIndicator(lookbackPeriods)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<MyResult> GetIndicator(
        this IEnumerable<(DateTime, double)> priceTuples,
        int lookbackPeriods) => priceTuples
            .ToSortedCollection()
            .CalcIndicator(lookbackPeriods);

    internal static List<MyResult> CalcIndicator(
        this Collection<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }

        // initialize
        List<MyResult> results = new(tpList.Count);

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double _) = tpList[i];

            MyResult result = new(date);
            results.Add(result);

            if (i + 1 >= lookbackPeriods)
            {
                double sumSma = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    sumSma += pValue;
                }

                result.Sma = (sumSma / lookbackPeriods).NaN2Null();
            }
        }

        return results;
    }
}

[TestClass]
public class CustomIndicatorTests
{
    private static readonly CultureInfo EnglishCulture = new("en-US", false);

#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1304 // Non-private readonly fields should begin with upper-case letter
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
    internal static readonly IEnumerable<Quote> quotes = TestData.GetDefault();
    internal static readonly IEnumerable<Quote> otherQuotes = TestData.GetCompare();
    internal static readonly IEnumerable<Quote> badQuotes = TestData.GetBad();
    internal static readonly IEnumerable<Quote> bigQuotes = TestData.GetTooBig();
    internal static readonly IEnumerable<Quote> maxQuotes = TestData.GetMax();
    internal static readonly IEnumerable<Quote> longishQuotes = TestData.GetLongish();
    internal static readonly IEnumerable<Quote> longestQuotes = TestData.GetLongest();
    internal static readonly IEnumerable<Quote> mismatchQuotes = TestData.GetMismatch();
    internal static readonly IEnumerable<Quote> noquotes = new List<Quote>();
    internal static readonly IEnumerable<Quote> onequote = TestData.GetDefault(1);
    internal static readonly IEnumerable<Quote> randomQuotes = TestData.GetRandom(1000);
    internal static readonly IEnumerable<Quote> zeroesQuotes = TestData.GetZeros();
    internal static readonly IEnumerable<(DateTime, double)> tupleNanny = TestData.GetTupleNaN();
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
#pragma warning restore SA1304 // Non-private readonly fields should begin with upper-case letter
#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter

    [TestMethod]
    public void Standard()
    {
        List<MyResult> results = quotes
            .GetIndicator(20)
            .ToList();

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
        List<MyResult> results = quotes
            .Use(CandlePart.Open)
            .GetIndicator(20)
            .ToList();

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
        List<MyResult> results = quotes
            .Use(CandlePart.Volume)
            .GetIndicator(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Sma != null));

        // sample values
        MyResult r24 = results[24];
        Assert.AreEqual(77293768.2, r24.Sma);

        MyResult r290 = results[290];
        Assert.AreEqual(157958070.8, r290.Sma);

        MyResult r501 = results[501];
        Assert.AreEqual(DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture), r501.Date);
        Assert.AreEqual(163695200, r501.Sma);
    }

    [TestMethod]
    public void Chainor()
    {
        List<EmaResult> results = quotes
            .GetIndicator(10)
            .GetEma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Ema != null));
    }

    [TestMethod]
    public void QuoteToSortedList()
    {
        IEnumerable<Quote> mismatch = TestData.GetMismatch();

        Collection<Quote> h = mismatch.ToSortedCollection();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(firstDate, h[0].Date);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Date);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[50].Date);
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<MyResult> r = tupleNanny
            .GetIndicator(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Sma is double and double.NaN));
    }

    [TestMethod]
    public void NaN()
    {
        List<MyResult> r = TestData.GetBtcUsdNan()
            .GetIndicator(50)
            .ToList();

        Assert.AreEqual(0, r.Count(x => x.Sma is double and double.NaN));
    }

    [TestMethod]
    public void BadData()
    {
        List<MyResult> r = badQuotes
            .GetIndicator(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Sma is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotesExist()
    {
        List<MyResult> r0 = noquotes
            .GetIndicator(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<MyResult> r1 = onequote
            .GetIndicator(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<MyResult> results = quotes
            .GetIndicator(20)
            .RemoveWarmupPeriods(19)
            .ToList();

        Assert.AreEqual(502 - 19, results.Count);
        Assert.AreEqual(251.8600, Math.Round(results.LastOrDefault().Sma.Value, 4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetIndicator(0));
}
