using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class AtrStop : TestBase
{
    [TestMethod]
    public void Standard()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        List<AtrStopResult> results = quotes
            .GetAtrStop(lookbackPeriods, multiplier, EndType.Close)
            .ToList();

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.AtrStop != null));

        // sample values
        AtrStopResult r1 = results[19];
        Assert.AreEqual(null, r1.AtrStop);
        Assert.AreEqual(null, r1.BuyStop);
        Assert.AreEqual(null, r1.SellStop);

        AtrStopResult r2 = results[20];
        Assert.AreEqual(210.9014m, NullMath.Round(r2.AtrStop, 4));
        Assert.AreEqual(null, r2.BuyStop);
        Assert.AreEqual(r2.AtrStop, r2.SellStop);

        AtrStopResult r3 = results[151];
        Assert.AreEqual(232.7860m, NullMath.Round(r3.AtrStop, 4));
        Assert.AreEqual(null, r3.BuyStop);
        Assert.AreEqual(r3.AtrStop, r3.SellStop);

        AtrStopResult r4 = results[152];
        Assert.AreEqual(236.3914m, NullMath.Round(r4.AtrStop, 4));
        Assert.AreEqual(r4.AtrStop, r4.BuyStop);
        Assert.AreEqual(null, r4.SellStop);

        AtrStopResult r5 = results[249];
        Assert.AreEqual(253.8863m, NullMath.Round(r5.AtrStop, 4));
        Assert.AreEqual(null, r5.BuyStop);
        Assert.AreEqual(r5.AtrStop, r5.SellStop);

        AtrStopResult r6 = results[501];
        Assert.AreEqual(246.3232m, NullMath.Round(r6.AtrStop, 4));
        Assert.AreEqual(r6.AtrStop, r6.BuyStop);
        Assert.AreEqual(null, r6.SellStop);
    }

    [TestMethod]
    public void HighLow()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        List<AtrStopResult> results = quotes
            .GetAtrStop(lookbackPeriods, multiplier, EndType.HighLow)
            .ToList();

        foreach (AtrStopResult r in results)
        {
            Console.WriteLine($"{r.Date:d},{r.BuyStop:N8},{r.SellStop:N8},{r.AtrStop:N8}");
        }

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.AtrStop != null));

        // sample values
        AtrStopResult r1 = results[12];
        Assert.AreEqual(null, r1.AtrStop);
        Assert.AreEqual(null, r1.BuyStop);
        Assert.AreEqual(null, r1.SellStop);

        AtrStopResult r2 = results[13];
        Assert.AreEqual(209.5436m, NullMath.Round(r2.AtrStop, 4));
        Assert.AreEqual(null, r2.BuyStop);
        Assert.AreEqual(r2.AtrStop, r2.SellStop);

        AtrStopResult r3 = results[151];
        Assert.AreEqual(232.8519m, NullMath.Round(r3.AtrStop, 4));
        Assert.AreEqual(null, r3.BuyStop);
        Assert.AreEqual(r3.AtrStop, r3.SellStop);

        AtrStopResult r4 = results[152];
        Assert.AreEqual(237.6436m, NullMath.Round(r4.AtrStop, 4));
        Assert.AreEqual(r4.AtrStop, r4.BuyStop);
        Assert.AreEqual(null, r4.SellStop);

        AtrStopResult r5 = results[249];
        Assert.AreEqual(253.8008m, NullMath.Round(r5.AtrStop, 4));
        Assert.AreEqual(null, r5.BuyStop);
        Assert.AreEqual(r5.AtrStop, r5.SellStop);

        AtrStopResult r6 = results[501];
        Assert.AreEqual(250.7954m, NullMath.Round(r6.AtrStop, 4));
        Assert.AreEqual(r6.AtrStop, r6.BuyStop);
        Assert.AreEqual(null, r6.SellStop);
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AtrStopResult> r = Indicator.GetAtrStop(badQuotes, 7);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<AtrStopResult> r0 = noquotes.GetAtrStop();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<AtrStopResult> r1 = onequote.GetAtrStop();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        List<AtrStopResult> results =
            quotes.GetAtrStop(lookbackPeriods, multiplier)
             .Condense()
             .ToList();

        // assertions
        Assert.AreEqual(482, results.Count);

        AtrStopResult last = results.LastOrDefault();
        Assert.AreEqual(246.3232m, NullMath.Round(last.AtrStop, 4));
        Assert.AreEqual(last.AtrStop, last.BuyStop);
        Assert.AreEqual(null, last.SellStop);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        List<AtrStopResult> results =
            quotes.GetAtrStop(lookbackPeriods, multiplier)
             .RemoveWarmupPeriods()
             .ToList();

        // assertions
        Assert.AreEqual(482, results.Count);

        AtrStopResult last = results.LastOrDefault();
        Assert.AreEqual(246.3232m, NullMath.Round(last.AtrStop, 4));
        Assert.AreEqual(last.AtrStop, last.BuyStop);
        Assert.AreEqual(null, last.SellStop);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetAtrStop(1));

        // bad multiplier
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetAtrStop(7, 0));
    }
}
