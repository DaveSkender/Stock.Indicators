using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class EmaTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<EmaResult> results = quotes
            .GetEma(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Ema != null));

        // sample values
        EmaResult r29 = results[29];
        Assert.AreEqual(216.6228, r29.Ema.Round(4));

        EmaResult r249 = results[249];
        Assert.AreEqual(255.3873, r249.Ema.Round(4));

        EmaResult r501 = results[501];
        Assert.AreEqual(249.3519, r501.Ema.Round(4));
    }

    [TestMethod]
    public void UseTuple()
    {
        List<EmaResult> results = quotes
            .Use(CandlePart.Close)
            .GetEma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Ema != null));
        Assert.AreEqual(0, results.Count(x => x.Ema is double and double.NaN));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<EmaResult> r = tupleNanny
            .GetEma(6)
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Ema is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<EmaResult> results = quotes
            .GetSma(2)
            .GetEma(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Ema != null));
        Assert.AreEqual(0, results.Count(x => x.Ema is double and double.NaN));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetEma(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
        Assert.AreEqual(0, results.Count(x => x.Sma is double and double.NaN));
    }

    [TestMethod]
    public void Stream()
    {
        List<Quote> quotesList = quotes
            .OrderBy(x => x.Date)
            .ToList();

        // time-series
        List<EmaResult> series = quotesList.GetEma(20).ToList();

        // stream simulation
        EmaBase emaBase = quotesList.Take(25).InitEma(20);

        for (int i = 25; i < series.Count; i++)
        {
            Quote q = quotesList[i];
            emaBase.Add(q);
            emaBase.Add(q); // redundant
        }

        List<EmaResult> stream = emaBase.Results.ToList();

        // assertions
        for (int i = 0; i < series.Count; i++)
        {
            EmaResult t = series[i];
            EmaResult s = stream[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Ema, s.Ema);
        }
    }

    [TestMethod]
    public void Chaining()
    {
        List<EmaResult> results = quotes
            .GetRsi(14)
            .GetEma(20)
            .ToList();

        // assertions
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(469, results.Count(x => x.Ema != null));
        Assert.AreEqual(0, results.Count(x => x.Ema is double and double.NaN));

        // sample values
        EmaResult r32 = results[32];
        Assert.IsNull(r32.Ema);

        EmaResult r33 = results[33];
        Assert.AreEqual(67.4565, r33.Ema.Round(4));

        EmaResult r249 = results[249];
        Assert.AreEqual(70.4659, r249.Ema.Round(4));

        EmaResult r501 = results[501];
        Assert.AreEqual(37.0728, r501.Ema.Round(4));
    }

    [TestMethod]
    public void Custom()
    {
        List<EmaResult> results = quotes
            .Use(CandlePart.Open)
            .GetEma(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Ema != null));

        // sample values
        EmaResult r29 = results[29];
        Assert.AreEqual(216.2643, r29.Ema.Round(4));

        EmaResult r249 = results[249];
        Assert.AreEqual(255.4875, r249.Ema.Round(4));

        EmaResult r501 = results[501];
        Assert.AreEqual(249.9157, r501.Ema.Round(4));
    }

    [TestMethod]
    public void BadData()
    {
        List<EmaResult> r = badQuotes
            .GetEma(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Ema is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<EmaResult> r0 = noquotes
            .GetEma(10)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<EmaResult> r1 = onequote
            .GetEma(10)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<EmaResult> results = quotes
            .GetEma(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (20 + 100), results.Count);

        EmaResult last = results.LastOrDefault();
        Assert.AreEqual(249.3519, last.Ema.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetEma(0));

        // null quote added
        EmaBase emaBase = quotes.InitEma(14);
        Assert.ThrowsException<InvalidQuotesException>(() =>
        emaBase.Add(null));
    }
}
