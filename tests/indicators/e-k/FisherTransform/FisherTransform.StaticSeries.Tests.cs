namespace StaticSeries;

[TestClass]
public class FisherTransform : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<FisherTransformResult> sut = Quotes
            .ToFisherTransform();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Fisher != 0).Should().HaveCount(501);

        // sample values
        sut[0].Fisher.Should().Be(0);
        sut[0].Trigger.Should().BeNull();

        Assert.AreEqual(0.3428, Math.Round(sut[1].Fisher, 4));
        sut[1].Trigger.Should().Be(0);

        Assert.AreEqual(0.6873, Math.Round(sut[2].Fisher, 4));
        (sut[2].Trigger.Value).Should().BeApproximately(0.3428, Money4);

        Assert.AreEqual(1.3324, Math.Round(sut[9].Fisher, 4));
        (sut[9].Trigger.Value).Should().BeApproximately(1.4704, Money4);

        Assert.AreEqual(0.9790, Math.Round(sut[10].Fisher, 4));
        (sut[10].Trigger.Value).Should().BeApproximately(1.3324, Money4);

        Assert.AreEqual(6.1509, Math.Round(sut[35].Fisher, 4));
        (sut[35].Trigger.Value).Should().BeApproximately(4.7014, Money4);

        Assert.AreEqual(5.4455, Math.Round(sut[36].Fisher, 4));
        (sut[36].Trigger.Value).Should().BeApproximately(6.1509, Money4);

        Assert.AreEqual(1.0349, Math.Round(sut[149].Fisher, 4));
        (sut[149].Trigger.Value).Should().BeApproximately(0.7351, Money4);

        Assert.AreEqual(1.3496, Math.Round(sut[249].Fisher, 4));
        (sut[249].Trigger.Value).Should().BeApproximately(1.4408, Money4);

        Assert.AreEqual(-1.2876, Math.Round(sut[501].Fisher, 4));
        (sut[501].Trigger.Value).Should().BeApproximately(-2.0071, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<FisherTransformResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToFisherTransform();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Fisher != 0).Should().HaveCount(501);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<FisherTransformResult> sut = Quotes
            .ToSma(2)
            .ToFisherTransform();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Fisher != 0).Should().HaveCount(501);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToFisherTransform()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(493);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<FisherTransformResult> r = BadQuotes
            .ToFisherTransform(9);

        r.Should().HaveCount(502);
        r.Where(static x => x.Fisher is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<FisherTransformResult> r0 = Noquotes
            .ToFisherTransform();

        r0.Should().BeEmpty();

        IReadOnlyList<FisherTransformResult> r1 = Onequote
            .ToFisherTransform();

        r1.Should().HaveCount(1);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToFisherTransform(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
