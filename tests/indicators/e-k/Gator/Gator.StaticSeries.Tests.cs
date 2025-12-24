namespace StaticSeries;

[TestClass]
public class Gator : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<GatorResult> sut = Quotes
            .ToGator();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Upper != null).Should().HaveCount(482);
        sut.Where(static x => x.Lower != null).Should().HaveCount(490);
        sut.Where(static x => x.UpperIsExpanding != null).Should().HaveCount(481);
        sut.Where(static x => x.LowerIsExpanding != null).Should().HaveCount(489);

        // sample values
        GatorResult r11 = sut[11];
        r11.Upper.Should().BeNull();
        r11.Lower.Should().BeNull();
        r11.UpperIsExpanding.Should().BeNull();
        r11.LowerIsExpanding.Should().BeNull();

        GatorResult r12 = sut[12];
        r12.Upper.Should().BeNull();
        Assert.AreEqual(-0.1402, Math.Round(r12.Lower.Value, 4));
        r12.UpperIsExpanding.Should().BeNull();
        r12.LowerIsExpanding.Should().BeNull();

        GatorResult r13 = sut[13];
        r13.Upper.Should().BeNull();
        Assert.AreEqual(-0.0406, Math.Round(r13.Lower.Value, 4));
        r13.UpperIsExpanding.Should().BeNull();
        Assert.IsFalse(r13.LowerIsExpanding);

        GatorResult r19 = sut[19];
        r19.Upper.Should().BeNull();
        Assert.AreEqual(-1.0018, Math.Round(r19.Lower.Value, 4));
        r19.UpperIsExpanding.Should().BeNull();
        Assert.IsTrue(r19.LowerIsExpanding);

        GatorResult r20 = sut[20];
        Assert.AreEqual(0.4004, Math.Round(r20.Upper.Value, 4));
        Assert.AreEqual(-1.0130, Math.Round(r20.Lower.Value, 4));
        r20.UpperIsExpanding.Should().BeNull();
        Assert.IsTrue(r20.LowerIsExpanding);

        GatorResult r21 = sut[21];
        Assert.AreEqual(0.7298, Math.Round(r21.Upper.Value, 4));
        Assert.AreEqual(-0.6072, Math.Round(r21.Lower.Value, 4));
        Assert.IsTrue(r21.UpperIsExpanding);
        Assert.IsFalse(r21.LowerIsExpanding);

        GatorResult r99 = sut[99];
        Assert.AreEqual(0.5159, Math.Round(r99.Upper.Value, 4));
        Assert.AreEqual(-0.2320, Math.Round(r99.Lower.Value, 4));
        Assert.IsFalse(r99.UpperIsExpanding);
        Assert.IsTrue(r99.LowerIsExpanding);

        GatorResult r249 = sut[249];
        Assert.AreEqual(3.1317, Math.Round(r249.Upper.Value, 4));
        Assert.AreEqual(-1.8058, Math.Round(r249.Lower.Value, 4));
        Assert.IsTrue(r249.UpperIsExpanding);
        Assert.IsFalse(r249.LowerIsExpanding);

        GatorResult r501 = sut[501];
        Assert.AreEqual(7.4538, Math.Round(r501.Upper.Value, 4));
        Assert.AreEqual(-9.2399, Math.Round(r501.Lower.Value, 4));
        Assert.IsTrue(r501.UpperIsExpanding);
        Assert.IsTrue(r501.LowerIsExpanding);
    }

    [TestMethod]
    public void FromAlligator()
    {
        IReadOnlyList<GatorResult> sut = Quotes
            .ToAlligator()
            .ToGator();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Upper != null).Should().HaveCount(482);
        sut.Where(static x => x.Lower != null).Should().HaveCount(490);
        sut.Where(static x => x.UpperIsExpanding != null).Should().HaveCount(481);
        sut.Where(static x => x.LowerIsExpanding != null).Should().HaveCount(489);

        // sample values
        GatorResult r11 = sut[11];
        r11.Upper.Should().BeNull();
        r11.Lower.Should().BeNull();
        r11.UpperIsExpanding.Should().BeNull();
        r11.LowerIsExpanding.Should().BeNull();

        GatorResult r12 = sut[12];
        r12.Upper.Should().BeNull();
        Assert.AreEqual(-0.1402, Math.Round(r12.Lower.Value, 4));
        r12.UpperIsExpanding.Should().BeNull();
        r12.LowerIsExpanding.Should().BeNull();

        GatorResult r13 = sut[13];
        r13.Upper.Should().BeNull();
        Assert.AreEqual(-0.0406, Math.Round(r13.Lower.Value, 4));
        r13.UpperIsExpanding.Should().BeNull();
        Assert.IsFalse(r13.LowerIsExpanding);

        GatorResult r19 = sut[19];
        r19.Upper.Should().BeNull();
        Assert.AreEqual(-1.0018, Math.Round(r19.Lower.Value, 4));
        r19.UpperIsExpanding.Should().BeNull();
        Assert.IsTrue(r19.LowerIsExpanding);

        GatorResult r20 = sut[20];
        Assert.AreEqual(0.4004, Math.Round(r20.Upper.Value, 4));
        Assert.AreEqual(-1.0130, Math.Round(r20.Lower.Value, 4));
        r20.UpperIsExpanding.Should().BeNull();
        Assert.IsTrue(r20.LowerIsExpanding);

        GatorResult r21 = sut[21];
        Assert.AreEqual(0.7298, Math.Round(r21.Upper.Value, 4));
        Assert.AreEqual(-0.6072, Math.Round(r21.Lower.Value, 4));
        Assert.IsTrue(r21.UpperIsExpanding);
        Assert.IsFalse(r21.LowerIsExpanding);

        GatorResult r99 = sut[99];
        Assert.AreEqual(0.5159, Math.Round(r99.Upper.Value, 4));
        Assert.AreEqual(-0.2320, Math.Round(r99.Lower.Value, 4));
        Assert.IsFalse(r99.UpperIsExpanding);
        Assert.IsTrue(r99.LowerIsExpanding);

        GatorResult r249 = sut[249];
        Assert.AreEqual(3.1317, Math.Round(r249.Upper.Value, 4));
        Assert.AreEqual(-1.8058, Math.Round(r249.Lower.Value, 4));
        Assert.IsTrue(r249.UpperIsExpanding);
        Assert.IsFalse(r249.LowerIsExpanding);

        GatorResult r501 = sut[501];
        Assert.AreEqual(7.4538, Math.Round(r501.Upper.Value, 4));
        Assert.AreEqual(-9.2399, Math.Round(r501.Lower.Value, 4));
        Assert.IsTrue(r501.UpperIsExpanding);
        Assert.IsTrue(r501.LowerIsExpanding);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<GatorResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToGator();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Upper != null).Should().HaveCount(482);
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<GatorResult> sut = Quotes
            .ToSma(2)
            .ToGator();

        sut.Should().HaveCount(502);
        sut.Where(static x => x.Upper != null).Should().HaveCount(481);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<GatorResult> r = BadQuotes
            .ToGator();

        r.Should().HaveCount(502);
        Assert.IsEmpty(r.Where(static x => x.Upper is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<GatorResult> r0 = Noquotes
            .ToGator();

        r0.Should().BeEmpty();

        IReadOnlyList<GatorResult> r1 = Onequote
            .ToGator();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<GatorResult> sut = Quotes
            .ToGator()
            .Condense();

        // assertions
        sut.Should().HaveCount(490);

        GatorResult last = sut[^1];
        Assert.AreEqual(7.4538, Math.Round(last.Upper.Value, 4));
        Assert.AreEqual(-9.2399, Math.Round(last.Lower.Value, 4));
        Assert.IsTrue(last.UpperIsExpanding);
        Assert.IsTrue(last.LowerIsExpanding);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<GatorResult> sut = Quotes
            .ToGator()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 150);

        GatorResult last = sut[^1];
        Assert.AreEqual(7.4538, Math.Round(last.Upper.Value, 4));
        Assert.AreEqual(-9.2399, Math.Round(last.Lower.Value, 4));
        Assert.IsTrue(last.UpperIsExpanding);
        Assert.IsTrue(last.LowerIsExpanding);
    }
}
