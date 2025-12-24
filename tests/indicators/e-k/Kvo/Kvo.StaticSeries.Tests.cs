namespace StaticSeries;

[TestClass]
public class Kvo : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<KvoResult> sut =
            Quotes.ToKvo();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Oscillator != null).Should().HaveCount(446);
        sut.Where(static x => x.Signal != null).Should().HaveCount(434);

        // sample values
        KvoResult r55 = sut[55];
        r55.Oscillator.Should().BeNull();
        r55.Signal.Should().BeNull();

        KvoResult r56 = sut[56];
        Assert.AreEqual(-2138454001, Math.Round(r56.Oscillator.Value, 0));
        r56.Signal.Should().BeNull();

        KvoResult r57 = sut[57];
        Assert.AreEqual(-2265495450, Math.Round(r57.Oscillator.Value, 0));
        r57.Signal.Should().BeNull();

        KvoResult r68 = sut[68];
        Assert.AreEqual(-1241548491, Math.Round(r68.Oscillator.Value, 0));
        Assert.AreEqual(-1489659254, Math.Round(r68.Signal.Value, 0));

        KvoResult r149 = sut[149];
        Assert.AreEqual(-62800843, Math.Round(r149.Oscillator.Value, 0));
        Assert.AreEqual(-18678832, Math.Round(r149.Signal.Value, 0));

        KvoResult r249 = sut[249];
        Assert.AreEqual(-51541005, Math.Round(r249.Oscillator.Value, 0));
        Assert.AreEqual(135207969, Math.Round(r249.Signal.Value, 0));

        KvoResult r501 = sut[501];
        Assert.AreEqual(-539224047, Math.Round(r501.Oscillator.Value, 0));
        Assert.AreEqual(-1548306127, Math.Round(r501.Signal.Value, 0));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToKvo()
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(437);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<KvoResult> r = BadQuotes
            .ToKvo();

        r.Should().HaveCount(502);
        Assert.IsEmpty(r.Where(static x => x.Oscillator is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<KvoResult> r0 = Noquotes
            .ToKvo();

        r0.Should().BeEmpty();

        IReadOnlyList<KvoResult> r1 = Onequote
            .ToKvo();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<KvoResult> sut = Quotes
            .ToKvo()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - (55 + 150));

        KvoResult last = sut[^1];
        Assert.AreEqual(-539224047, Math.Round(last.Oscillator.Value, 0));
        Assert.AreEqual(-1548306127, Math.Round(last.Signal.Value, 0));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKvo(2));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKvo(20, 20));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToKvo(34, 55, 0));
    }
}
