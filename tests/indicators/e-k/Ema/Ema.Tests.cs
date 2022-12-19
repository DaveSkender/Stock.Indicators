using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

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
        Assert.AreEqual(216.6228, NullMath.Round(r29.Ema, 4));

        EmaResult r249 = results[249];
        Assert.AreEqual(255.3873, NullMath.Round(r249.Ema, 4));

        EmaResult r501 = results[501];
        Assert.AreEqual(249.3519, NullMath.Round(r501.Ema, 4));
    }

    [TestMethod]
    public void UsePart()
    {
        List<EmaResult> results = quotes
            .Use(CandlePart.Open)
            .GetEma(20)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Ema != null));

        // sample values
        EmaResult r29 = results[29];
        Assert.AreEqual(216.2643, NullMath.Round(r29.Ema, 4));

        EmaResult r249 = results[249];
        Assert.AreEqual(255.4875, NullMath.Round(r249.Ema, 4));

        EmaResult r501 = results[501];
        Assert.AreEqual(249.9157, NullMath.Round(r501.Ema, 4));
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
    public void ChaineeMore()
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
        Assert.AreEqual(67.4565, NullMath.Round(r33.Ema, 4));

        EmaResult r249 = results[249];
        Assert.AreEqual(70.4659, NullMath.Round(r249.Ema, 4));

        EmaResult r501 = results[501];
        Assert.AreEqual(37.0728, NullMath.Round(r501.Ema, 4));
    }

    [TestMethod]
    public void StreamInitBase()
    {
        List<Quote> quotesList = quotes
            .OrderBy(x => x.Date)
            .ToList();

        // time-series, for comparison
        List<EmaResult> series = quotesList.GetEma(20).ToList();

        // stream simulation
        EmaBase emaBase = quotesList.Take(25).InitEma(20);
        int[] dups = new int[] { 33, 67, 111, 250, 251 };

        for (int i = 25; i < series.Count; i++)
        {
            Quote q = quotesList[i];
            emaBase.Add(q);

            // duplicate value
            if (dups.Contains(i))
            {
                emaBase.Add(q);
            }
        }

        List<EmaResult> stream = emaBase.Results.ToList();

        // assert, should equal series
        for (int i = 0; i < series.Count; i++)
        {
            EmaResult t = series[i];
            EmaResult s = stream[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Ema, s.Ema);
        }
    }

    [TestMethod]
    public void StreamInitEmpty()
    {
        List<Quote> quotesList = quotes
            .OrderBy(x => x.Date)
            .ToList();

        // time-series, for comparison
        List<EmaResult> series = quotesList
          .GetEma(20)
          .ToList();

        // stream simulation
        EmaBase emaBase = new(20);
        int[] dups = new int[] { 3, 7, 11, 250, 251 };

        for (int i = 0; i < series.Count; i++)
        {
            Quote q = quotesList[i];
            emaBase.Add(q);

            // duplicate value
            if (dups.Contains(i))
            {
                emaBase.Add(q);
            }
        }

        List<EmaResult> stream = emaBase
          .Results
          .ToList();

        // assert, should equal series
        for (int i = 0; i < series.Count; i++)
        {
            EmaResult t = series[i];
            EmaResult s = stream[i];

            Assert.AreEqual(t.Date, s.Date);
            Assert.AreEqual(t.Ema, s.Ema);
        }
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
        Assert.AreEqual(249.3519, NullMath.Round(last.Ema, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
          => quotes.GetEma(0));

        // null quote added
        EmaBase emaBase = quotes.InitEma(14);

        Assert.ThrowsException<InvalidQuotesException>(()
          => emaBase.Add(null));
    }
}
