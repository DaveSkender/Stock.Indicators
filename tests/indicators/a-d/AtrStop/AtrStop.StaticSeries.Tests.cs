namespace StaticSeries;

[TestClass]
public class AtrStop : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        IReadOnlyList<AtrStopResult> results = Quotes
            .ToAtrStop(lookbackPeriods, multiplier);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(481, results.Where(x => x.AtrStop != null));

        // sample values
        AtrStopResult r20 = results[20];
        Assert.IsNull(r20.AtrStop);
        Assert.IsNull(r20.BuyStop);
        Assert.IsNull(r20.SellStop);

        AtrStopResult r21 = results[21];
        Assert.AreEqual(211.13, r21.AtrStop.Round(4));
        Assert.IsNull(r21.BuyStop);
        Assert.AreEqual(r21.AtrStop, r21.SellStop);

        AtrStopResult r151 = results[151];
        Assert.AreEqual(232.7861, r151.AtrStop.Round(4));
        Assert.IsNull(r151.BuyStop);
        Assert.AreEqual(r151.AtrStop, r151.SellStop);

        AtrStopResult r152 = results[152];
        Assert.AreEqual(236.3913, r152.AtrStop.Round(4));
        Assert.AreEqual(r152.AtrStop, r152.BuyStop);
        Assert.IsNull(r152.SellStop);

        AtrStopResult r249 = results[249];
        Assert.AreEqual(253.8863, r249.AtrStop.Round(4));
        Assert.IsNull(r249.BuyStop);
        Assert.AreEqual(r249.AtrStop, r249.SellStop);

        AtrStopResult r501 = results[501];
        Assert.AreEqual(246.3232, r501.AtrStop.Round(4));
        Assert.AreEqual(r501.AtrStop, r501.BuyStop);
        Assert.IsNull(r501.SellStop);
    }

    [TestMethod]
    public void HighLow()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        IReadOnlyList<AtrStopResult> results = Quotes
            .ToAtrStop(lookbackPeriods, multiplier, EndType.HighLow);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(481, results.Where(x => x.AtrStop != null));

        // sample values
        AtrStopResult r20 = results[20];
        Assert.IsNull(r20.AtrStop);
        Assert.IsNull(r20.BuyStop);
        Assert.IsNull(r20.SellStop);

        AtrStopResult r21 = results[21];
        Assert.AreEqual(210.23, r21.AtrStop.Round(4));
        Assert.IsNull(r21.BuyStop);
        Assert.AreEqual(r21.AtrStop, r21.SellStop);

        AtrStopResult r69 = results[69];
        Assert.AreEqual(221.0594, r69.AtrStop.Round(4));
        Assert.IsNull(r69.BuyStop);
        Assert.AreEqual(r69.AtrStop, r69.SellStop);

        AtrStopResult r70 = results[70];
        Assert.AreEqual(226.4624, r70.AtrStop.Round(4));
        Assert.AreEqual(r70.AtrStop, r70.BuyStop);
        Assert.IsNull(r70.SellStop);

        AtrStopResult r249 = results[249];
        Assert.AreEqual(253.4863, r249.AtrStop.Round(4));
        Assert.IsNull(r249.BuyStop);
        Assert.AreEqual(r249.AtrStop, r249.SellStop);

        AtrStopResult r501 = results[501];
        Assert.AreEqual(252.6932, r501.AtrStop.Round(4));
        Assert.AreEqual(r501.AtrStop, r501.BuyStop);
        Assert.IsNull(r501.SellStop);
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<AtrStopResult> r = BadQuotes
            .ToAtrStop(7);

        Assert.HasCount(502, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<AtrStopResult> r0 = Noquotes
            .ToAtrStop();

        Assert.IsEmpty(r0);

        IReadOnlyList<AtrStopResult> r1 = Onequote
            .ToAtrStop();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        IReadOnlyList<AtrStopResult> results = Quotes
            .ToAtrStop(lookbackPeriods, multiplier)
            .Condense();

        // assertions
        Assert.HasCount(481, results);

        AtrStopResult last = results[^1];
        Assert.AreEqual(246.3232, last.AtrStop.Round(4));
        Assert.AreEqual(last.AtrStop, last.BuyStop);
        Assert.IsNull(last.SellStop);
    }

    [TestMethod]
    public void Removed()
    {
        int lookbackPeriods = 21;
        double multiplier = 3;

        IReadOnlyList<AtrStopResult> results = Quotes
            .ToAtrStop(lookbackPeriods, multiplier)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(481, results);

        AtrStopResult last = results[^1];
        Assert.AreEqual(246.3232, last.AtrStop.Round(4));
        Assert.AreEqual(last.AtrStop, last.BuyStop);
        Assert.IsNull(last.SellStop);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToAtrStop(1));

        // bad multiplier
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToAtrStop(7, 0));
    }
}
