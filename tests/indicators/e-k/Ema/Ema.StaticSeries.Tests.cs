namespace StaticSeries;

[TestClass]
public class EmaTests : StaticSeriesTestBase
{
    [TestMethod]
    public void Increment()
    {
        double ema = Ema.Increment(20, 217.5693, 222.10);

        ema.Round(4).Should().Be(218.0008);
    }

    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<EmaResult> results = Quotes.ToEma(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Ema != null));

        // sample values
        EmaResult r29 = results[29];
        r29.Ema.Round(4).Should().Be(216.6228);

        EmaResult r249 = results[249];
        r249.Ema.Round(4).Should().Be(255.3873);

        EmaResult r501 = results[501];
        r501.Ema.Round(4).Should().Be(249.3519);
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
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Ema != null));

        // sample values
        EmaResult r29 = results[29];
        r29.Ema.Round(4).Should().Be(216.2643);

        EmaResult r249 = results[249];
        r249.Ema.Round(4).Should().Be(255.4875);

        EmaResult r501 = results[501];
        r501.Ema.Round(4).Should().Be(249.9157);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToEma(20);

        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Ema != null));
        Assert.IsEmpty(results.Where(static x => x.Ema is double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .ToSma(2)
            .ToEma(20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Ema != null));
        Assert.IsEmpty(results.Where(static x => x.Ema is double.NaN));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToEma(20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(474, results.Where(static x => x.Sma != null));
        Assert.IsEmpty(results.Where(static x => x.Sma is double.NaN));
    }

    [TestMethod]
    public void ChaineeMore()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .ToRsi()
            .ToEma(20);

        // assertions
        Assert.HasCount(502, results);
        Assert.HasCount(469, results.Where(static x => x.Ema != null));
        Assert.IsEmpty(results.Where(static x => x.Ema is double.NaN));

        // sample values
        EmaResult r32 = results[32];
        Assert.IsNull(r32.Ema);

        EmaResult r33 = results[33];
        r33.Ema.Round(4).Should().Be(67.4565);

        EmaResult r249 = results[249];
        r249.Ema.Round(4).Should().Be(70.4659);

        EmaResult r501 = results[501];
        r501.Ema.Round(4).Should().Be(37.0728);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<EmaResult> r = BadQuotes.ToEma(15);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Ema is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<EmaResult> r0 = Noquotes.ToEma(10);

        Assert.IsEmpty(r0);

        IReadOnlyList<EmaResult> r1 = Onequote.ToEma(10);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<EmaResult> results = Quotes.ToEma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (20 + 100), results);

        EmaResult last = results[^1];
        last.Ema.Round(4).Should().Be(249.3519);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToEma(0));
}
