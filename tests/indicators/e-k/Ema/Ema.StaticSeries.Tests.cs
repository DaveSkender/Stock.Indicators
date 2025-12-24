namespace StaticSeries;

[TestClass]
public class EmaTests : StaticSeriesTestBase
{
    [TestMethod]
    public void Increment()
    {
        double ema = Ema.Increment(20, 217.5693, 222.10);

        ema.Should().BeApproximately(218.0008, Money4);
    }

    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<EmaResult> results = Quotes.ToEma(20);

        // proper quantities
        results.Should().HaveCount(502);
        results.Where(static x => x.Ema != null).Should().HaveCount(483);

        // sample values
        results[29].Ema.Should().BeApproximately(216.6228, Money4);
        results[249].Ema.Should().BeApproximately(255.3873, Money4);
        results[501].Ema.Should().BeApproximately(249.3519, Money4);
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
        results.Should().HaveCount(502);
        results.Where(static x => x.Ema != null).Should().HaveCount(483);

        // sample values
        results[29].Ema.Should().BeApproximately(216.2643, Money4);
        results[249].Ema.Should().BeApproximately(255.4875, Money4);
        results[501].Ema.Should().BeApproximately(249.9157, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .Use(CandlePart.Close)
            .ToEma(20);

        results.Should().HaveCount(502);
        results.Where(static x => x.Ema != null).Should().HaveCount(483);
        results.Where(static x => x.Ema is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .ToSma(2)
            .ToEma(20);

        results.Should().HaveCount(502);
        results.Where(static x => x.Ema != null).Should().HaveCount(482);
        results.Where(static x => x.Ema is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToEma(20)
            .ToSma(10);

        results.Should().HaveCount(502);
        results.Where(static x => x.Sma != null).Should().HaveCount(474);
        results.Where(static x => x.Sma is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public void ChaineeMore()
    {
        IReadOnlyList<EmaResult> results = Quotes
            .ToRsi()
            .ToEma(20);

        // assertions
        results.Should().HaveCount(502);
        results.Where(static x => x.Ema != null).Should().HaveCount(469);
        results.Where(static x => x.Ema is double.NaN).Should().BeEmpty();

        // sample values
        results[32].Ema.Should().BeNull();
        results[33].Ema.Should().BeApproximately(67.4565, Money4);
        results[249].Ema.Should().BeApproximately(70.4659, Money4);
        results[501].Ema.Should().BeApproximately(37.0728, Money4);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<EmaResult> r = BadQuotes.ToEma(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Ema is double.NaN).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<EmaResult> r0 = Noquotes.ToEma(10);

        r0.Should().BeEmpty();

        IReadOnlyList<EmaResult> r1 = Onequote.ToEma(10);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<EmaResult> results = Quotes.ToEma(20)
            .RemoveWarmupPeriods();

        // assertions
        results.Should().HaveCount(502 - (20 + 100));
        results[^1].Ema.Should().BeApproximately(249.3519, Money4);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToEma(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
