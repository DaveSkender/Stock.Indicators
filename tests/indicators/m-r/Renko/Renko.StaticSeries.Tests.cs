namespace StaticSeries;

[TestClass]
public class Renko : StaticSeriesTestBase
{
    /// <summary>
    /// close
    /// </summary>
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<RenkoResult> sut = Quotes
            .ToRenko(2.5m);

        // assertions

        sut.Should().HaveCount(112);
        sut.Where(static x => x.IsUp).Should().HaveCount(62);
        sut.Where(static x => !x.IsUp).Should().HaveCount(50);

        // sample values
        RenkoResult r0 = sut[0];
        r0.Open.Should().Be(213m);
        r0.High.Should().Be(216.89m);
        r0.Low.Should().Be(212.53m);
        r0.Close.Should().Be(215.5m);
        r0.Volume.Should().Be(1180981564m);
        Assert.IsTrue(r0.IsUp);

        RenkoResult r5 = sut[5];
        r5.Open.Should().Be(225.5m);
        r5.High.Should().Be(228.15m);
        r5.Low.Should().Be(219.77m);
        r5.Close.Should().Be(228m);
        r5.Volume.Should().Be(4192959240m);
        Assert.IsTrue(r5.IsUp);

        RenkoResult last = sut[^1];
        last.Open.Should().Be(240.5m);
        last.High.Should().Be(243.68m);
        last.Low.Should().Be(234.52m);
        last.Close.Should().Be(243m);
        last.Volume.Should().Be(189794032m);
        Assert.IsTrue(last.IsUp);
    }

    [TestMethod]
    public void StandardHighLow()
    {
        IReadOnlyList<RenkoResult> sut = Quotes
            .ToRenko(2.5m, EndType.HighLow);

        // assertions

        sut.Should().HaveCount(159);

        // sample values
        RenkoResult r0 = sut[0];
        r0.Open.Should().Be(213m);
        r0.High.Should().Be(216.89m);
        r0.Low.Should().Be(212.53m);
        r0.Close.Should().Be(215.5m);
        Assert.AreEqual(1180981564m, r0.Volume.Round(0));
        Assert.IsTrue(r0.IsUp);

        RenkoResult r25 = sut[25];
        r25.Open.Should().Be(270.5m);
        r25.High.Should().Be(273.16m);
        r25.Low.Should().Be(271.96m);
        r25.Close.Should().Be(273m);
        Assert.AreEqual(100801672m, r25.Volume.Round(0));
        Assert.IsTrue(r25.IsUp);

        RenkoResult last = sut[^1];
        last.Open.Should().Be(243m);
        last.High.Should().Be(246.73m);
        last.Low.Should().Be(241.87m);
        last.Close.Should().Be(245.5m);
        Assert.AreEqual(51999637m, last.Volume.Round(0));
        Assert.IsTrue(last.IsUp);
    }

    [TestMethod]
    public void Atr()
    {
        IReadOnlyList<RenkoResult> sut = Quotes
            .ToRenkoAtr(14);

        // proper quantities
        sut.Should().HaveCount(29);

        // sample values
        RenkoResult r0 = sut[0];
        ((double?)r0.Open).Should().BeApproximately((double)212.8m, Money3);
        ((double?)r0.High).Should().BeApproximately((double)220.19m, Money3);
        ((double?)r0.Low).Should().BeApproximately((double)212.53m, Money3);
        ((double?)r0.Close).Should().BeApproximately((double)218.9497m, Money3);
        Assert.AreEqual(2090292272m, r0.Volume.Round(0));
        Assert.IsTrue(r0.IsUp);

        RenkoResult last = sut[^1];
        ((double?)last.Open).Should().BeApproximately((double)237.3990m, Money3);
        ((double?)last.High).Should().BeApproximately((double)246.73m, Money3);
        ((double?)last.Low).Should().BeApproximately((double)229.42m, Money3);
        ((double?)last.Close).Should().BeApproximately((double)243.5487m, Money3);
        Assert.AreEqual(715446448m, last.Volume.Round(0));
        Assert.IsTrue(last.IsUp);
    }

    [TestMethod]
    public void UseAsQuotes()
    {
        IReadOnlyList<RenkoResult> renkoQuotes = Quotes.ToRenko(2.5m);
        IReadOnlyList<SmaResult> renkoSma = renkoQuotes.ToSma(5);
        renkoSma.Where(static x => x.Sma != null).Should().HaveCount(108);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<RenkoResult> r = BadQuotes
            .ToRenko(100m);

        Assert.AreNotEqual(0, r.Count);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<RenkoResult> r0 = Noquotes
            .ToRenko(0.01m);

        r0.Should().BeEmpty();
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad arguments
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRenko(0));

        // bad end type
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRenko(2, (EndType)int.MaxValue));
    }
}
