namespace StaticSeries;

[TestClass]
public class Alma : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int lookbackPeriods = 10;
        const double offset = 0.85;
        const double sigma = 6;

        IReadOnlyList<AlmaResult> results = Quotes
            .ToAlma(lookbackPeriods, offset, sigma);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Alma != null));

        // sample values
        AlmaResult r1 = results[8];
        Assert.IsNull(r1.Alma);

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
            .ToAlma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Alma != null));

        AlmaResult last = results[^1];
        Assert.AreEqual(242.1871, last.Alma.Round(4));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<AlmaResult> results = Quotes
            .ToSma(2)
            .ToAlma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(492, results.Where(static x => x.Alma != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        const int lookbackPeriods = 10;
        const double offset = 0.85;
        const double sigma = 6;

        IReadOnlyList<SmaResult> results = Quotes
            .ToAlma(lookbackPeriods, offset, sigma)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(484, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public void NaN()
    {
        IReadOnlyList<AlmaResult> r1
            = Data.GetBtcUsdNan().ToAlma();

        Assert.IsEmpty(r1.Where(static x => x.Alma is double.NaN));

        IReadOnlyList<AlmaResult> r2
            = Data.GetBtcUsdNan().ToAlma(20);

        Assert.IsEmpty(r2.Where(static x => x.Alma is double.NaN));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<AlmaResult> r = BadQuotes.ToAlma(14, 0.5, 3);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Alma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<AlmaResult> r0 = Noquotes.ToAlma();

        Assert.IsEmpty(r0);

        IReadOnlyList<AlmaResult> r1 = Onequote.ToAlma();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AlmaResult> results = Quotes
            .ToAlma(10)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 9, results);

        AlmaResult last = results[^1];
        Assert.AreEqual(242.1871, last.Alma.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToAlma(0, 1, 5));

        // bad offset
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToAlma(15, 1.1, 3));

        // bad sigma
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToAlma(10, 0.5, 0));
    }
}
