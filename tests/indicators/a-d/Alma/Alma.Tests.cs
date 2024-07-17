namespace Series;

[TestClass]
public class Alma : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        int lookbackPeriods = 10;
        double offset = 0.85;
        double sigma = 6;

        IReadOnlyList<AlmaResult> results = Quotes
            .GetAlma(lookbackPeriods, offset, sigma);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Alma != null));

        // sample values
        AlmaResult r1 = results[8];
        Assert.AreEqual(null, r1.Alma);

        AlmaResult r2 = results[9];
        Assert.AreEqual(214.1839, r2.Alma.Round(4));

        AlmaResult r3 = results[24];
        Assert.AreEqual(216.0619, r3.Alma.Round(4));

        AlmaResult r4 = results[149];
        Assert.AreEqual(235.8609, r4.Alma.Round(4));

        AlmaResult r5 = results[249];
        Assert.AreEqual(257.5787, r5.Alma.Round(4));

        AlmaResult r6 = results[501];
        Assert.AreEqual(242.1871, r6.Alma.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<AlmaResult> results = Quotes
            .Use(CandlePart.Close)
            .GetAlma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(493, results.Count(x => x.Alma != null));

        AlmaResult last = results[^1];
        Assert.AreEqual(242.1871, last.Alma.Round(4));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<AlmaResult> results = Quotes
            .GetSma(2)
            .GetAlma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(492, results.Count(x => x.Alma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        int lookbackPeriods = 10;
        double offset = 0.85;
        double sigma = 6;

        IReadOnlyList<SmaResult> results = Quotes
            .GetAlma(lookbackPeriods, offset, sigma)
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(484, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<AlmaResult> r1
            = Data.GetBtcUsdNan().GetAlma();

        Assert.AreEqual(0, r1.Count(x => x.Alma is double.NaN));

        IReadOnlyList<AlmaResult> r2
            = Data.GetBtcUsdNan().GetAlma(20);

        Assert.AreEqual(0, r2.Count(x => x.Alma is double.NaN));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<AlmaResult> r = BadQuotes.GetAlma(14, 0.5, 3);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Alma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<AlmaResult> r0 = Noquotes.GetAlma();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<AlmaResult> r1 = Onequote.GetAlma();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AlmaResult> results = Quotes
            .GetAlma(10)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 9, results.Count);

        AlmaResult last = results[^1];
        Assert.AreEqual(242.1871, last.Alma.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetAlma(0, 1, 5));

        // bad offset
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetAlma(15, 1.1, 3));

        // bad sigma
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetAlma(10, 0.5, 0));
    }
}
