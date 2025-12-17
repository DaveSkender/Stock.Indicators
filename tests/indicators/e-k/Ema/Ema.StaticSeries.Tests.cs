using Tests.Tools;

namespace StaticSeries;

[TestClass]
public class EmaTests : StaticSeriesTestBase
{
    [TestMethod]
    public void Increment()
    {
        double ema = Ema.Increment(20, 217.5693, 222.10);

        ema.Should().BeApproximately(218.0008, TestPrecision.Money4);
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
        r29.Ema.Should().BeApproximately(216.6228, TestPrecision.Money4);

        EmaResult r249 = results[249];
        r249.Ema.Should().BeApproximately(255.3873, TestPrecision.Money4);

        EmaResult r501 = results[501];
        r501.Ema.Should().BeApproximately(249.3519, TestPrecision.Money4);
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
        r29.Ema.Should().BeApproximately(216.2643, TestPrecision.Money4);

        EmaResult r249 = results[249];
        r249.Ema.Should().BeApproximately(255.4875, TestPrecision.Money4);

        EmaResult r501 = results[501];
        r501.Ema.Should().BeApproximately(249.9157, TestPrecision.Money4);
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
        Assert.AreEqual(67.4565, r33.Ema.Round(4));

        EmaResult r249 = results[249];
        Assert.AreEqual(70.4659, r249.Ema.Round(4));

        EmaResult r501 = results[501];
        Assert.AreEqual(37.0728, r501.Ema.Round(4));
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
        Assert.AreEqual(249.3519, last.Ema.Round(4));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToEma(0));
}
