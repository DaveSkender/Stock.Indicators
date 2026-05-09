namespace StaticSeries;

[TestClass]
public class Smma : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<SmmaResult> sut = Quotes
            .ToSmma(20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Smma != null).Should().HaveCount(483);

        // starting calculations at proper index
        sut[18].Smma.Should().BeNull();
        sut[19].Smma.Should().NotBeNull();

        // sample values
        Assert.AreEqual(214.52500, Math.Round(sut[19].Smma.Value, 5));
        Assert.AreEqual(214.55125, Math.Round(sut[20].Smma.Value, 5));
        Assert.AreEqual(214.58319, Math.Round(sut[21].Smma.Value, 5));
        Assert.AreEqual(225.78071, Math.Round(sut[100].Smma.Value, 5));
        Assert.AreEqual(255.67462, Math.Round(sut[501].Smma.Value, 5));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<SmmaResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToSmma(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Smma != null).Should().HaveCount(483);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<SmmaResult> sut = Quotes
            .ToSma(2)
            .ToSmma(20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Smma != null).Should().HaveCount(482);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToSmma(20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(474);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<SmmaResult> r = BadQuotes
            .ToSmma(15);

        r.Should().HaveCount(502);
        r.Where(static x => x.Smma is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<SmmaResult> r0 = Noquotes
            .ToSmma(5);

        r0.Should().BeEmpty();

        IReadOnlyList<SmmaResult> r1 = Onequote
            .ToSmma(5);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<SmmaResult> sut = Quotes
            .ToSmma(20)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - (20 + 100));
        Assert.AreEqual(255.67462, Math.Round(sut[^1].Smma.Value, 5));
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToSmma(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
