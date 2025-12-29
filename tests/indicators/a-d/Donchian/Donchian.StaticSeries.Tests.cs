namespace StaticSeries;

[TestClass]
public class Donchian : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<DonchianResult> sut = Quotes
            .ToDonchian();

        // proper quantities
        sut.Should().HaveCount(502);
        sut.Where(static x => x.Centerline != null).Should().HaveCount(482);
        sut.Where(static x => x.UpperBand != null).Should().HaveCount(482);
        sut.Where(static x => x.LowerBand != null).Should().HaveCount(482);
        sut.Where(static x => x.Width != null).Should().HaveCount(482);

        // sample values
        DonchianResult r1 = sut[19];
        r1.Centerline.Should().BeNull();
        r1.UpperBand.Should().BeNull();
        r1.LowerBand.Should().BeNull();
        r1.Width.Should().BeNull();

        DonchianResult r2 = sut[20];
        ((double?)r2.Centerline).Should().BeApproximately((double)214.2700m, Money3);
        ((double?)r2.UpperBand).Should().BeApproximately((double)217.0200m, Money3);
        ((double?)r2.LowerBand).Should().BeApproximately((double)211.5200m, Money3);
        ((double?)r2.Width).Should().BeApproximately((double)0.025669m, Money6);

        DonchianResult r3 = sut[249];
        ((double?)r3.Centerline).Should().BeApproximately((double)254.2850m, Money3);
        ((double?)r3.UpperBand).Should().BeApproximately((double)258.7000m, Money3);
        ((double?)r3.LowerBand).Should().BeApproximately((double)249.8700m, Money3);
        ((double?)r3.Width).Should().BeApproximately((double)0.034725m, Money6);

        DonchianResult r4 = sut[485];
        ((double?)r4.Centerline).Should().BeApproximately((double)265.5350m, Money3);
        ((double?)r4.UpperBand).Should().BeApproximately((double)274.3900m, Money3);
        ((double?)r4.LowerBand).Should().BeApproximately((double)256.6800m, Money3);
        ((double?)r4.Width).Should().BeApproximately((double)0.066696m, Money6);

        DonchianResult r5 = sut[501];
        ((double?)r5.Centerline).Should().BeApproximately((double)251.5050m, Money3);
        ((double?)r5.UpperBand).Should().BeApproximately((double)273.5900m, Money3);
        ((double?)r5.LowerBand).Should().BeApproximately((double)229.4200m, Money3);
        ((double?)r5.Width).Should().BeApproximately((double)0.175623m, Money6);
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<DonchianResult> r = BadQuotes
            .ToDonchian(15);

        r.Should().HaveCount(502);
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<DonchianResult> r0 = Noquotes
            .ToDonchian();

        r0.Should().BeEmpty();

        IReadOnlyList<DonchianResult> r1 = Onequote
            .ToDonchian();

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<DonchianResult> sut = Quotes
            .ToDonchian()
            .Condense();

        // assertions
        sut.Should().HaveCount(502 - 20);

        DonchianResult last = sut[^1];
        ((double?)last.Centerline).Should().BeApproximately((double)251.5050m, Money3);
        ((double?)last.UpperBand).Should().BeApproximately((double)273.5900m, Money3);
        ((double?)last.LowerBand).Should().BeApproximately((double)229.4200m, Money3);
        ((double?)last.Width).Should().BeApproximately((double)0.175623m, Money6);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<DonchianResult> sut = Quotes
            .ToDonchian()
            .RemoveWarmupPeriods();

        // assertions
        sut.Should().HaveCount(502 - 20);

        DonchianResult last = sut[^1];
        ((double?)last.Centerline).Should().BeApproximately((double)251.5050m, Money3);
        ((double?)last.UpperBand).Should().BeApproximately((double)273.5900m, Money3);
        ((double?)last.LowerBand).Should().BeApproximately((double)229.4200m, Money3);
        ((double?)last.Width).Should().BeApproximately((double)0.175623m, Money6);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => FluentActions
            .Invoking(static () => Quotes.ToDonchian(0))
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
}
