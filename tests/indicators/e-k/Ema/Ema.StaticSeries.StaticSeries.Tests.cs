namespace StaticSeries;

[TestClass]
public class EmaTests : StaticSeriesTestBase
{
    [TestMethod]
    public void Increment()
    {
        double ema = Ema.Increment(20, 217.5693, 222.10);

        Assert.AreEqual(218.0008, ema.Round(4));
    }

    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .ToEma(20);

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
    public void UsePart()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .Use(CandlePart.Open)
            .ToEma(20);

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
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
    public void UseReusable()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToEma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Ema != null));
        Assert.AreEqual(0, results.Count(x => x.Ema is double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .GetSma(2)
            .ToEma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Ema != null));
        Assert.AreEqual(0, results.Count(x => x.Ema is double.NaN));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToEma(20)
            .GetSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
        Assert.AreEqual(0, results.Count(x => x.Sma is double.NaN));
    }

    [TestMethod]
    public void ChaineeMore()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .GetRsi()
            .ToEma(20);

        // assertions
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(469, results.Count(x => x.Ema != null));
        Assert.AreEqual(0, results.Count(x => x.Ema is double.NaN));

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
    public override void BadData()
    {
        IReadOnlyList<EmaResult> r = BadQuotes
            .ToEma(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Ema is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<EmaResult> r0 = Noquotes
            .ToEma(10);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<EmaResult> r1 = Onequote
            .ToEma(10);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .ToEma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - (20 + 100), results.Count);

        EmaResult last = results[^1];
        Assert.AreEqual(249.3519, last.Ema.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.ToEma(0));
}
