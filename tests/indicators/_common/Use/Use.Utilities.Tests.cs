using Test.Data;

namespace StaticSeries;

[TestClass]
public class UseUtilitiesTests
{
    private static IReadOnlyList<IQuote> Quotes => Data.GetDefault();

    [TestMethod]
    public void MaEnvelopes_Use_ReturnsChainableValues()
    {
        // Calculate MA Envelopes
        IReadOnlyList<MaEnvelopeResult> results = Quotes.ToMaEnvelopes(20, 2.5);

        // Test each field
        IReadOnlyList<QuotePart> centerline = results.Use(MaEnvelopeField.Centerline);
        IReadOnlyList<QuotePart> upperEnvelope = results.Use(MaEnvelopeField.UpperEnvelope);
        IReadOnlyList<QuotePart> lowerEnvelope = results.Use(MaEnvelopeField.LowerEnvelope);

        // Assert proper count
        centerline.Should().HaveCount(502);
        upperEnvelope.Should().HaveCount(502);
        lowerEnvelope.Should().HaveCount(502);

        // Assert values are correct for non-null results
        MaEnvelopeResult lastResult = results[501];
        QuotePart lastCenterline = centerline[501];
        QuotePart lastUpper = upperEnvelope[501];
        QuotePart lastLower = lowerEnvelope[501];

        lastCenterline.Value.Should().BeApproximately(lastResult.Centerline!.Value, 0.0001);
        lastUpper.Value.Should().BeApproximately(lastResult.UpperEnvelope!.Value, 0.0001);
        lastLower.Value.Should().BeApproximately(lastResult.LowerEnvelope!.Value, 0.0001);

        // Assert default is Centerline
        IReadOnlyList<QuotePart> defaultField = results.Use();
        defaultField[501].Value.Should().Be(centerline[501].Value);
    }

    [TestMethod]
    public void Alligator_Use_ReturnsChainableValues()
    {
        IReadOnlyList<AlligatorResult> results = Quotes.ToAlligator();

        IReadOnlyList<QuotePart> jaw = results.Use(AlligatorLine.Jaw);
        IReadOnlyList<QuotePart> teeth = results.Use(AlligatorLine.Teeth);
        IReadOnlyList<QuotePart> lips = results.Use(AlligatorLine.Lips);

        jaw.Should().HaveCount(502);
        teeth.Should().HaveCount(502);
        lips.Should().HaveCount(502);

        // Verify non-null values match
        AlligatorResult lastResult = results[501];
        lastResult.Jaw.Should().NotBeNull();
        jaw[501].Value.Should().BeApproximately(lastResult.Jaw!.Value, 0.0001);
    }

    [TestMethod]
    public void AtrStop_Use_ReturnsChainableValues()
    {
        IReadOnlyList<AtrStopResult> results = Quotes.ToAtrStop(21, 3);

        IReadOnlyList<QuotePart> atrStop = results.Use(AtrStopField.AtrStop);
        IReadOnlyList<QuotePart> buyStop = results.Use(AtrStopField.BuyStop);
        IReadOnlyList<QuotePart> sellStop = results.Use(AtrStopField.SellStop);
        IReadOnlyList<QuotePart> atr = results.Use(AtrStopField.Atr);

        atrStop.Should().HaveCount(502);
        buyStop.Should().HaveCount(502);
        sellStop.Should().HaveCount(502);
        atr.Should().HaveCount(502);
    }

    [TestMethod]
    public void Donchian_Use_ReturnsChainableValues()
    {
        IReadOnlyList<DonchianResult> results = Quotes.ToDonchian(20);

        IReadOnlyList<QuotePart> centerline = results.Use(DonchianField.Centerline);
        IReadOnlyList<QuotePart> upperBand = results.Use(DonchianField.UpperBand);
        IReadOnlyList<QuotePart> lowerBand = results.Use(DonchianField.LowerBand);
        IReadOnlyList<QuotePart> width = results.Use(DonchianField.Width);

        centerline.Should().HaveCount(502);
        upperBand.Should().HaveCount(502);
        lowerBand.Should().HaveCount(502);
        width.Should().HaveCount(502);

        // Verify default is Centerline (enum value 0)
        IReadOnlyList<QuotePart> defaultField = results.Use();
        defaultField[501].Value.Should().Be(centerline[501].Value);
    }

    [TestMethod]
    public void Fcb_Use_ReturnsChainableValues()
    {
        IReadOnlyList<FcbResult> results = Quotes.ToFcb(2);

        IReadOnlyList<QuotePart> upperBand = results.Use(FcbField.UpperBand);
        IReadOnlyList<QuotePart> lowerBand = results.Use(FcbField.LowerBand);

        upperBand.Should().HaveCount(502);
        lowerBand.Should().HaveCount(502);
    }

    [TestMethod]
    public void Fractal_Use_ReturnsChainableValues()
    {
        IReadOnlyList<FractalResult> results = Quotes.ToFractal(5);

        IReadOnlyList<QuotePart> bear = results.Use(FractalSide.Bear);
        IReadOnlyList<QuotePart> bull = results.Use(FractalSide.Bull);

        bear.Should().HaveCount(502);
        bull.Should().HaveCount(502);
    }

    [TestMethod]
    public void Gator_Use_ReturnsChainableValues()
    {
        IReadOnlyList<GatorResult> results = Quotes.ToGator();

        IReadOnlyList<QuotePart> upper = results.Use(GatorSide.Upper);
        IReadOnlyList<QuotePart> lower = results.Use(GatorSide.Lower);

        upper.Should().HaveCount(502);
        lower.Should().HaveCount(502);
    }

    [TestMethod]
    public void Ichimoku_Use_ReturnsChainableValues()
    {
        IReadOnlyList<IchimokuResult> results = Quotes.ToIchimoku();

        IReadOnlyList<QuotePart> tenkanSen = results.Use(IchimokuLine.TenkanSen);
        IReadOnlyList<QuotePart> kijunSen = results.Use(IchimokuLine.KijunSen);
        IReadOnlyList<QuotePart> senkouSpanA = results.Use(IchimokuLine.SenkouSpanA);
        IReadOnlyList<QuotePart> senkouSpanB = results.Use(IchimokuLine.SenkouSpanB);
        IReadOnlyList<QuotePart> chikouSpan = results.Use(IchimokuLine.ChikouSpan);

        tenkanSen.Should().HaveCount(502);
        kijunSen.Should().HaveCount(502);
        senkouSpanA.Should().HaveCount(502);
        senkouSpanB.Should().HaveCount(502);
        chikouSpan.Should().HaveCount(502);
    }

    [TestMethod]
    public void Keltner_Use_ReturnsChainableValues()
    {
        IReadOnlyList<KeltnerResult> results = Quotes.ToKeltner(20, 2, 10);

        IReadOnlyList<QuotePart> centerline = results.Use(KeltnerField.Centerline);
        IReadOnlyList<QuotePart> upperBand = results.Use(KeltnerField.UpperBand);
        IReadOnlyList<QuotePart> lowerBand = results.Use(KeltnerField.LowerBand);
        IReadOnlyList<QuotePart> width = results.Use(KeltnerField.Width);

        centerline.Should().HaveCount(502);
        upperBand.Should().HaveCount(502);
        lowerBand.Should().HaveCount(502);
        width.Should().HaveCount(502);

        // Verify default is Centerline (enum value 0)
        IReadOnlyList<QuotePart> defaultField = results.Use();
        defaultField[501].Value.Should().Be(centerline[501].Value);
    }

    [TestMethod]
    public void Pivots_Use_ReturnsChainableValues()
    {
        IReadOnlyList<PivotsResult> results = Quotes.ToPivots(2, 2, 20);

        IReadOnlyList<QuotePart> highPoint = results.Use(PivotPointField.HighPoint);
        IReadOnlyList<QuotePart> lowPoint = results.Use(PivotPointField.LowPoint);
        IReadOnlyList<QuotePart> highLine = results.Use(PivotPointField.HighLine);
        IReadOnlyList<QuotePart> lowLine = results.Use(PivotPointField.LowLine);

        highPoint.Should().HaveCount(502);
        lowPoint.Should().HaveCount(502);
        highLine.Should().HaveCount(502);
        lowLine.Should().HaveCount(502);
    }

    [TestMethod]
    public void PivotPoints_Use_ReturnsChainableValues()
    {
        IReadOnlyList<PivotPointsResult> results = Quotes.ToPivotPoints(PeriodSize.Month, PivotPointType.Standard);

        IReadOnlyList<QuotePart> pp = results.Use(PivotField.PP);
        IReadOnlyList<QuotePart> s1 = results.Use(PivotField.S1);
        IReadOnlyList<QuotePart> r1 = results.Use(PivotField.R1);

        pp.Should().HaveCount(502);
        s1.Should().HaveCount(502);
        r1.Should().HaveCount(502);

        // Verify default is PP (enum value 0)
        IReadOnlyList<QuotePart> defaultField = results.Use();
        defaultField[501].Value.Should().Be(pp[501].Value);
    }

    [TestMethod]
    public void RollingPivots_Use_ReturnsChainableValues()
    {
        IReadOnlyList<RollingPivotsResult> results = Quotes.ToRollingPivots(11, 9, PivotPointType.Standard);

        IReadOnlyList<QuotePart> pp = results.Use(PivotField.PP);
        IReadOnlyList<QuotePart> s1 = results.Use(PivotField.S1);
        IReadOnlyList<QuotePart> r1 = results.Use(PivotField.R1);

        pp.Should().HaveCount(502);
        s1.Should().HaveCount(502);
        r1.Should().HaveCount(502);

        // Verify default is PP (enum value 0)
        IReadOnlyList<QuotePart> defaultField = results.Use();
        defaultField[501].Value.Should().Be(pp[501].Value);
    }

    [TestMethod]
    public void StarcBands_Use_ReturnsChainableValues()
    {
        IReadOnlyList<StarcBandsResult> results = Quotes.ToStarcBands(20, 2, 10);

        IReadOnlyList<QuotePart> centerline = results.Use(StarcBandsField.Centerline);
        IReadOnlyList<QuotePart> upperBand = results.Use(StarcBandsField.UpperBand);
        IReadOnlyList<QuotePart> lowerBand = results.Use(StarcBandsField.LowerBand);

        centerline.Should().HaveCount(502);
        upperBand.Should().HaveCount(502);
        lowerBand.Should().HaveCount(502);

        // Verify default is Centerline (enum value 0)
        IReadOnlyList<QuotePart> defaultField = results.Use();
        defaultField[501].Value.Should().Be(centerline[501].Value);
    }

    [TestMethod]
    public void StdDevChannels_Use_ReturnsChainableValues()
    {
        IReadOnlyList<StdDevChannelsResult> results = Quotes.ToStdDevChannels(20, 2);

        IReadOnlyList<QuotePart> centerline = results.Use(StdDevChannelsField.Centerline);
        IReadOnlyList<QuotePart> upperChannel = results.Use(StdDevChannelsField.UpperChannel);
        IReadOnlyList<QuotePart> lowerChannel = results.Use(StdDevChannelsField.LowerChannel);

        centerline.Should().HaveCount(502);
        upperChannel.Should().HaveCount(502);
        lowerChannel.Should().HaveCount(502);

        // Verify default is Centerline (enum value 0)
        IReadOnlyList<QuotePart> defaultField = results.Use();
        defaultField[501].Value.Should().Be(centerline[501].Value);
    }

    [TestMethod]
    public void SuperTrend_Use_ReturnsChainableValues()
    {
        IReadOnlyList<SuperTrendResult> results = Quotes.ToSuperTrend(10, 3);

        IReadOnlyList<QuotePart> superTrend = results.Use(SuperTrendField.SuperTrend);
        IReadOnlyList<QuotePart> upperBand = results.Use(SuperTrendField.UpperBand);
        IReadOnlyList<QuotePart> lowerBand = results.Use(SuperTrendField.LowerBand);

        superTrend.Should().HaveCount(502);
        upperBand.Should().HaveCount(502);
        lowerBand.Should().HaveCount(502);

        // Verify default is SuperTrend (enum value 0)
        IReadOnlyList<QuotePart> defaultField = results.Use();
        defaultField[501].Value.Should().Be(superTrend[501].Value);
    }

    [TestMethod]
    public void Vortex_Use_ReturnsChainableValues()
    {
        IReadOnlyList<VortexResult> results = Quotes.ToVortex(14);

        IReadOnlyList<QuotePart> pvi = results.Use(VortexField.Pvi);
        IReadOnlyList<QuotePart> nvi = results.Use(VortexField.Nvi);

        pvi.Should().HaveCount(502);
        nvi.Should().HaveCount(502);
    }

    [TestMethod]
    public void CandleResult_Use_ReturnsChainableValues()
    {
        IReadOnlyList<CandleResult> results = Quotes.ToMarubozu();

        IReadOnlyList<QuotePart> prices = results.Use();

        prices.Should().HaveCount(502);

        // Verify Price field is extracted
        CandleResult lastResult = results[501];
        QuotePart lastPrice = prices[501];
        lastPrice.Value.Should().Be((double?)lastResult.Price ?? double.NaN);
    }

    [TestMethod]
    public void Use_CanBeChained()
    {
        // Test that Use() output can be chained to another indicator
        IReadOnlyList<SmaResult> chainedResult = Quotes
            .ToMaEnvelopes(20, 2.5)
            .Use(MaEnvelopeField.Centerline)
            .ToSma(10);

        chainedResult.Should().HaveCount(502);
        chainedResult.Where(static x => x.Sma != null).Should().NotBeEmpty();
    }
}
