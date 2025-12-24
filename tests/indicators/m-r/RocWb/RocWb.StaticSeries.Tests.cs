namespace StaticSeries;

[TestClass]
public class RocWb : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<RocWbResult> sut = Quotes
            .ToRocWb(20, 3, 20);

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Roc != null).Should().HaveCount(482);
        sut.Where(static x => x.RocEma != null).Should().HaveCount(480);
        sut.Where(static x => x.UpperBand != null).Should().HaveCount(463);
        sut.Where(static x => x.LowerBand != null).Should().HaveCount(463);

        // sample values
        RocWbResult r19 = sut[19];
        r19.Roc.Should().BeNull();
        r19.RocEma.Should().BeNull();
        r19.UpperBand.Should().BeNull();
        r19.LowerBand.Should().BeNull();

        RocWbResult r20 = sut[20];
        Assert.AreEqual(1.0573, Math.Round(r20.Roc.Value, 4));
        r20.RocEma.Should().BeNull();
        r20.UpperBand.Should().BeNull();
        r20.LowerBand.Should().BeNull();

        RocWbResult r22 = sut[22];
        Assert.AreEqual(0.9617, Math.Round(r22.RocEma.Value, 4));
        r22.UpperBand.Should().BeNull();
        r22.LowerBand.Should().BeNull();

        RocWbResult r23 = sut[23];
        Assert.AreEqual(0.8582, Math.Round(r23.RocEma.Value, 4));
        r23.UpperBand.Should().BeNull();
        r23.LowerBand.Should().BeNull();

        RocWbResult r38 = sut[38];
        Assert.AreEqual(3.6872, Math.Round(r38.RocEma.Value, 4));
        r38.UpperBand.Should().BeNull();
        r38.LowerBand.Should().BeNull();

        RocWbResult r39 = sut[39];
        Assert.AreEqual(4.5348, Math.Round(r39.RocEma.Value, 4));
        Assert.AreEqual(3.0359, Math.Round(r39.UpperBand.Value, 4));
        Assert.AreEqual(-3.0359, Math.Round(r39.LowerBand.Value, 4));

        RocWbResult r49 = sut[49];
        Assert.AreEqual(2.3147, Math.Round(r49.RocEma.Value, 4));
        Assert.AreEqual(3.6761, Math.Round(r49.UpperBand.Value, 4));

        RocWbResult r149 = sut[149];
        Assert.AreEqual(1.7377, Math.Round(r149.UpperBand.Value, 4));

        RocWbResult r249 = sut[249];
        Assert.AreEqual(3.0683, Math.Round(r249.UpperBand.Value, 4));

        RocWbResult r501 = sut[501];
        Assert.AreEqual(-8.2482, Math.Round(r501.Roc.Value, 4));
        Assert.AreEqual(-8.3390, Math.Round(r501.RocEma.Value, 4));
        Assert.AreEqual(6.1294, Math.Round(r501.UpperBand.Value, 4));
        Assert.AreEqual(-6.1294, Math.Round(r501.LowerBand.Value, 4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<RocWbResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToRocWb(20, 3, 20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Roc != null).Should().HaveCount(482);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<RocWbResult> sut = Quotes
            .ToSma(2)
            .ToRocWb(20, 3, 20);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Roc != null).Should().HaveCount(481);
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToRocWb(20, 3, 20)
            .ToSma(10);

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Sma != null).Should().HaveCount(473);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<RocWbResult> r = BadQuotes
            .ToRocWb(35, 3, 35);

        r.Should().HaveCount(502);
        Assert.IsEmpty(r.Where(static x => x.Roc is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<RocWbResult> r0 = Noquotes
            .ToRocWb(5, 3, 2);

        r0.Should().BeEmpty();

        IReadOnlyList<RocWbResult> r1 = Onequote
            .ToRocWb(5, 3, 2);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<RocWbResult> sut = Quotes
            .ToRocWb(20, 3, 20)
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - (20 + 3 + 100));

        RocWbResult last = sut[^1];
        Assert.AreEqual(-8.2482, Math.Round(last.Roc.Value, 4));
        Assert.AreEqual(-8.3390, Math.Round(last.RocEma.Value, 4));
        Assert.AreEqual(6.1294, Math.Round(last.UpperBand.Value, 4));
        Assert.AreEqual(-6.1294, Math.Round(last.LowerBand.Value, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRocWb(0, 3, 12));

        // bad EMA period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRocWb(14, 0, 14));

        // bad STDDEV period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRocWb(15, 3, 16));
    }
}
