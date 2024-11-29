using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591 // Missing XML comments

namespace Skender.Stock.Indicators;

// OBSOLETE IN v3.0.0
public static partial class Indicator
{
    // GENERAL INDICATOR METHODS
    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetAdl(..)` with `ToAdl(..)`", false)]
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
    this IEnumerable<TQuote> quotes)
    where TQuote : IQuote
    => quotes.ToSortedList().ToAdl();

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetAdx(..)` with `ToAdx(..)`", false)]
    public static IEnumerable<AdxResult> GetAdx<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 14)
        where TQuote : IQuote
        => quotes.ToSortedList().ToAdx(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetAlligator(..)` with `ToAlligator(..)`", false)]
    public static IEnumerable<AlligatorResult> GetAlligator<TQuote>(
        this IEnumerable<TQuote> quotes,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        where TQuote : IQuote
        => quotes.ToSortedList().ToAlligator(
            jawPeriods, jawOffset,
            teethPeriods, teethOffset,
            lipsPeriods, lipsOffset);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetAlma(..)` with `ToAlma(..)`", false)]
    public static IEnumerable<AlmaResult> GetAlma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        where TQuote : IQuote
        => quotes.ToSortedList().ToAlma(lookbackPeriods, offset, sigma);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetAroon(..)` with `ToAroon(..)`", false)]
    public static IEnumerable<AroonResult> GetAroon<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 25)
        where TQuote : IQuote
        => quotes.ToSortedList().ToAroon(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetAtr(..)` with `ToAtr(..)`", false)]
    public static IEnumerable<AtrResult> GetAtr<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 14)
        where TQuote : IQuote
        => quotes.ToSortedList().ToAtr(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetAtrStop(..)` with `ToAtrStop(..)`", false)]
    public static IEnumerable<AtrStopResult> GetAtrStop<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 21,
        double multiplier = 3,
        EndType endType = EndType.Close)
        where TQuote : IQuote
        => quotes.ToSortedList().ToAtrStop(lookbackPeriods, multiplier, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetAwesome(..)` with `ToAwesome(..)`", false)]
    public static IEnumerable<AwesomeResult> GetAwesome<TQuote>(
        this IEnumerable<TQuote> quotes, int fastPeriods = 5, int slowPeriods = 34)
        where TQuote : IQuote
        => quotes.ToSortedList().ToAwesome(fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetBeta(..)` with `ToBeta(..)`", false)]
    public static IEnumerable<BetaResult> GetBeta<TQuote>(
        this IEnumerable<TQuote> sourceEval,
        IEnumerable<TQuote> sourceMrkt,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        where TQuote : IQuote
        => sourceEval
            .ToSortedList()
            .ToBeta(sourceMrkt.ToSortedList(), lookbackPeriods, type);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetBollingerBands(..)` with `ToBollingerBands(..)`", false)]
    public static IEnumerable<BollingerBandsResult> GetBollingerBands<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToBollingerBands(
                lookbackPeriods,
                standardDeviations);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetBop(..)` with `ToBop(..)`", false)]
    public static IEnumerable<BopResult> GetBop<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 14)
        where TQuote : IQuote
        => quotes.ToSortedList().ToBop(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetCci(..)` with `ToCci(..)`", false)]
    public static IEnumerable<CciResult> GetCci<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 20)
        where TQuote : IQuote
        => quotes.ToSortedList().ToCci(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetChaikinOsc(..)` with `ToChaikinOsc(..)`", false)]
    public static IEnumerable<ChaikinOscResult> GetChaikinOsc<TQuote>(
        this IEnumerable<TQuote> quotes, int fastPeriods = 3, int slowPeriods = 10)
        where TQuote : IQuote
        => quotes.ToSortedList().ToChaikinOsc(fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetChandelier(..)` with `ToChandelier(..)`", false)]
    public static IEnumerable<ChandelierResult> GetChandelier<TQuote>(
        this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 22,
            double multiplier = 3,
            ChandelierType type = ChandelierType.Long)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToChandelier(lookbackPeriods, multiplier, type);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetChop(..)` with `ToChop(..)`", false)]
    public static IEnumerable<ChopResult> GetChop<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 14)
        where TQuote : IQuote
        => quotes.ToSortedList().ToChop(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetCmf(..)` with `ToCmf(..)`", false)]
    public static IEnumerable<CmfResult> GetCmf<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 20)
        where TQuote : IQuote
        => quotes.ToSortedList().ToCmf(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetCmo(..)` with `ToCmo(..)`", false)]
    public static IEnumerable<CmoResult> GetCmo<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToCmo(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetConnorsRsi(..)` with `ToConnorsRsi(..)`", false)]
    public static IEnumerable<ConnorsRsiResult> GetConnorsRsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int rsiPeriods = 3, int streakPeriods = 2, int rankPeriods = 100)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetCorrelation(..)` with `ToCorrelation(..)`", false)]
    public static IEnumerable<CorrResult> GetCorrelation<TQuote>(
        this IEnumerable<TQuote> sourceA,
        IEnumerable<TQuote> sourceB, int lookbackPeriods)
        where TQuote : IQuote
        => sourceA.ToSortedList().ToCorrelation(sourceB.ToSortedList(), lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetDema(..)` with `ToDema(..)`", false)]
    public static IEnumerable<DemaResult> GetDema<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToDema(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetDoji(..)` with `ToDoji(..)`", false)]
    public static IEnumerable<CandleResult> GetDoji<TQuote>(
        this IEnumerable<TQuote> quotes, double maxPriceChangePercent = 0.1)
        where TQuote : IQuote
        => quotes.ToSortedList().ToDoji(maxPriceChangePercent);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetDonchian(..)` with `ToDonchian(..)`", false)]
    public static IEnumerable<DonchianResult> GetDonchian<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 20)
        where TQuote : IQuote
        => quotes.ToSortedList().ToDonchian(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetDpo(..)` with `ToDpo(..)`", false)]
    public static IEnumerable<DpoResult> GetDpo<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToDpo(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetDynamic(..)` with `ToDynamic(..)`", false)]
    public static IEnumerable<DynamicResult> GetDynamic<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, double kFactor = 0.6)
        where TQuote : IQuote
        => quotes.ToSortedList().ToDynamic(lookbackPeriods, kFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetElderRay(..)` with `ToElderRay(..)`", false)]
    public static IEnumerable<ElderRayResult> GetElderRay<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 13)
        where TQuote : IQuote
        => quotes.ToSortedList().ToElderRay(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetEma(..)` with `ToEma(..)`", false)]
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToEma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetEpma(..)` with `ToEpma(..)`", false)]
    public static IEnumerable<EpmaResult> GetEpma<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToEpma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetFcb(..)` with `ToFcb(..)`", false)]
    public static IEnumerable<FcbResult> GetFcb<TQuote>(
        this IEnumerable<TQuote> quotes, int windowSpan = 2)
        where TQuote : IQuote
        => quotes.ToSortedList().ToFcb(windowSpan);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetFisherTransform(..)` with `ToFisherTransform(..)`", false)]
    public static IEnumerable<FisherTransformResult> GetFisherTransform<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 10)
        where TQuote : IQuote
        => quotes.ToSortedList().ToFisherTransform(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetForceIndex(..)` with `ToForceIndex(..)`", false)]
    public static IEnumerable<ForceIndexResult> GetForceIndex<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 2)
        where TQuote : IQuote
        => quotes.ToSortedList().ToForceIndex(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetFractal(..)` with `ToFractal(..)`", false)]
    public static IEnumerable<FractalResult> GetFractal<TQuote>(
        this IEnumerable<TQuote> quotes, int windowSpan = 2, EndType endType = EndType.HighLow)
        where TQuote : IQuote
        => quotes.ToSortedList().ToFractal(windowSpan, windowSpan, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetFractal(..)` with `ToFractal(..)`", false)]
    public static IEnumerable<FractalResult> GetFractal<TQuote>(
        this IEnumerable<TQuote> quotes, int leftSpan, int rightSpan, EndType endType = EndType.HighLow)
        where TQuote : IQuote
        => quotes.ToSortedList().ToFractal(leftSpan, rightSpan, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetGator(..)` with `ToGator(..)`", false)]
    public static IEnumerable<GatorResult> GetGator<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes.ToSortedList().ToGator();

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetHeikinAshi(..)` with `ToHeikinAshi(..)`", false)]
    public static IEnumerable<HeikinAshiResult> GetHeikinAshi<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes.ToSortedList().ToHeikinAshi();

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetHma(..)` with `ToHma(..)`", false)]
    public static IEnumerable<HmaResult> GetHma<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote => quotes.ToSortedList().ToHma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetHtTrendline(..)` with `ToHtTrendline(..)`", false)]
    public static IEnumerable<HtlResult> GetHtTrendline<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes.ToSortedList().ToHtTrendline();

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetHurst(..)` with `ToHurst(..)`", false)]
    public static IEnumerable<HurstResult> GetHurst<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 100)
        where TQuote : IQuote
        => quotes.ToSortedList().ToHurst(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetIchimoku(..)` with `ToIchimoku(..)`", false)]
    public static IEnumerable<IchimokuResult> GetIchimoku<TQuote>(
        this IEnumerable<TQuote> quotes,
            int tenkanPeriods = 9,
            int kijunPeriods = 26,
            int senkouBPeriods = 52)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetKama(..)` with `ToKama(..)`", false)]
    public static IEnumerable<KamaResult> GetKama<TQuote>(
        this IEnumerable<TQuote> quotes,
            int erPeriods = 10,
            int fastPeriods = 2,
            int slowPeriods = 30)
        where TQuote : IQuote
        => quotes.ToSortedList().ToKama(erPeriods, fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetKeltner(..)` with `ToKeltner(..)`", false)]
    public static IEnumerable<KeltnerResult> GetKeltner<TQuote>(
        this IEnumerable<TQuote> quotes,
        int emaPeriods = 20,
        int multiplier = 2,
        int atrPeriods = 10)
        where TQuote : IQuote
        => quotes.ToSortedList().ToKeltner(emaPeriods, multiplier, atrPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetKvo(..)` with `ToKvo(..)`", false)]
    public static IEnumerable<KvoResult> GetKvo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
        where TQuote : IQuote
        => quotes.ToSortedList()
        .ToKvo(fastPeriods, slowPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetMacd(..)` with `ToMacd(..)`", false)]
    public static IEnumerable<MacdResult> GetMacd<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where TQuote : IQuote
        => quotes.ToSortedList().ToMacd(fastPeriods, slowPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetMaEnvelopes(..)` with `ToMaEnvelopes(..)`", false)]
    public static IEnumerable<MaEnvelopeResult> GetMaEnvelopes<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetMama(..)` with `ToMama(..)`", false)]
    public static IEnumerable<MamaResult> GetMama<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToMama(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetMarubozu(..)` with `ToMarubozu(..)`", false)]
    public static IEnumerable<CandleResult> GetMarubozu<TQuote>(
        this IEnumerable<TQuote> quotes, double minBodyPercent = 95)
        where TQuote : IQuote
        => quotes.ToSortedList().ToMarubozu(minBodyPercent);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetMfi(..)` with `ToMfi(..)`", false)]
    public static IEnumerable<MfiResult> GetMfi<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 14)
        where TQuote : IQuote
        => quotes.ToSortedList().ToMfi(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetObv(..)` with `ToObv(..)`", false)]
    public static IEnumerable<ObvResult> GetObv<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes.ToSortedList().ToObv();

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetParabolicSar(..)` with `ToParabolicSar(..)`", false)]
    public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IEnumerable<TQuote> quotes,
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToParabolicSar(accelerationStep, maxAccelerationFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetParabolicSar(..)` with `ToParabolicSar(..)`", false)]
    public static IEnumerable<ParabolicSarResult> GetParabolicSar<TQuote>(
        this IEnumerable<TQuote> quotes,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToParabolicSar(accelerationStep, maxAccelerationFactor, initialFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetPivotPoints(..)` with `ToPivotPoints(..)`", false)]
    public static IEnumerable<PivotPointsResult> GetPivotPoints<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToPivotPoints((PeriodSize)lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetPivots(..)` with `ToPivots(..)`", false)]
    public static IEnumerable<PivotsResult> GetPivots<TQuote>(
        this IEnumerable<TQuote> quotes,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToPivots(leftSpan, rightSpan, maxTrendPeriods, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetPmo(..)` with `ToPmo(..)`", false)]
    public static IEnumerable<PmoResult> GetPmo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
        where TQuote : IQuote
        => quotes.ToSortedList().ToPmo(timePeriods, smoothPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetPrs(..)` with `ToPrs(..)`", false)]
    public static IEnumerable<PrsResult> GetPrs<TQuote>(
        this IEnumerable<TQuote> sourceBase,
        IEnumerable<TQuote> sourceEval, int? lookbackPeriods = null)
        where TQuote : IQuote
        => sourceBase.ToSortedList()
            .ToPrs(sourceEval.ToSortedList(), lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetPvo(..)` with `ToPvo(..)`", false)]
    public static IEnumerable<PvoResult> GetPvo<TQuote>(
        this IEnumerable<TQuote> quotes,
        int fastPeriods = 9, int slowPeriods = 12, int signalPeriods = 9)
        where TQuote : IQuote
        => quotes.ToSortedList().ToPvo(fastPeriods, slowPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetRenko(..)` with `ToRenko(..)`", false)]
    public static IEnumerable<RenkoResult> GetRenko<TQuote>(
        this IEnumerable<TQuote> quotes, decimal brickSize, EndType endType = EndType.Close)
        where TQuote : IQuote
        => quotes.ToSortedList().ToRenko(brickSize, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetRenkoAtr(..)` with `ToRenkoAtr(..)`", false)]
    public static IEnumerable<RenkoResult> GetRenkoAtr<TQuote>(
        this IEnumerable<TQuote> quotes, int atrPeriods, EndType endType = EndType.Close)
        where TQuote : IQuote
        => quotes.ToSortedList().ToRenkoAtr(atrPeriods, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetRoc(..)` with `ToRoc(..)`", false)]
    public static IEnumerable<RocResult> GetRoc<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToRoc(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetRocWb(..)` with `ToRocWb(..)`", false)]
    public static IEnumerable<RocWbResult> GetRocWb<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods, int emaPeriods, int stdDevPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetRollingPivots(..)` with `ToRollingPivots(..)`", false)]
    public static IEnumerable<RollingPivotsResult> GetRollingPivots<TQuote>(
        this IEnumerable<TQuote> quotes,
        int windowPeriods,
        int offsetPeriods,
        PivotPointType pointType = PivotPointType.Standard)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToRollingPivots(windowPeriods, offsetPeriods, pointType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetRsi(..)` with `ToRsi(..)`", false)]
    public static IEnumerable<RsiResult> GetRsi<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 14)
        where TQuote : IQuote
        => quotes.ToSortedList().ToRsi(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetSlope(..)` with `ToSlope(..)`", false)]
    public static IEnumerable<SlopeResult> GetSlope<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToSlope(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetSma(..)` with `ToSma(..)`", false)]
    public static IEnumerable<SmaResult> GetSma<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToSma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetSmaAnalysis(..)` with `ToSmaAnalysis(..)`", false)]
    public static IEnumerable<SmaAnalysis> GetSmaAnalysis<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToSmaAnalysis(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetSmi(..)` with `ToSmi(..)`", false)]
    public static IEnumerable<SmiResult> GetSmi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToSmi(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetSmma(..)` with `ToSmma(..)`", false)]
    public static IEnumerable<SmmaResult> GetSmma<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToSmma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetStarcBands(..)` with `ToStarcBands(..)`", false)]
    public static IEnumerable<StarcBandsResult> GetStarcBands<TQuote>(
        this IEnumerable<TQuote> quotes,
        int smaPeriods,
        double multiplier = 2,
        int atrPeriods = 10)
        where TQuote : IQuote
        => quotes.ToSortedList().ToStarcBands(smaPeriods, multiplier, atrPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetStc(..)` with `ToStc(..)`", false)]
    public static IEnumerable<StcResult> GetStc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        where TQuote : IQuote
        => quotes.ToSortedList().ToStc(cyclePeriods, fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetStdDev(..)` with `ToStdDev(..)`", false)]
    public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToStdDev(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetStdDevChannels(..)` with `ToStdDevChannels(..)`", false)]
    public static IEnumerable<StdDevChannelsResult> GetStdDevChannels<TQuote>(
        this IEnumerable<TQuote> quotes, int? lookbackPeriods = 20, double stdDeviations = 2)
        where TQuote : IQuote
        => quotes.ToSortedList().ToStdDevChannels(lookbackPeriods, stdDeviations);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetStoch(..)` with `ToStoch(..)`", false)]
    public static IEnumerable<StochResult> GetStoch<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
        where TQuote : IQuote
        => quotes.ToSortedList().ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetStoch(..)` with `ToStoch(..)`", false)]
    public static IEnumerable<StochResult> GetStoch<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int signalPeriods,
            int smoothPeriods,
            double kFactor,
            double dFactor,
            MaType movingAverageType)
        where TQuote : IQuote
        => quotes.ToSortedList().ToStoch(
            lookbackPeriods,
            signalPeriods,
            smoothPeriods,
            kFactor,
            dFactor,
            movingAverageType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetStochRsi(..)` with `ToStochRsi(..)`", false)]
    public static IEnumerable<StochRsiResult> GetStochRsi<TQuote>(
            this IEnumerable<TQuote> quotes,
            int rsiPeriods,
            int stochPeriods,
            int signalPeriods,
            int smoothPeriods = 1)
        where TQuote : IQuote
        => quotes.ToSortedList().ToStochRsi(
            rsiPeriods,
            stochPeriods,
            signalPeriods,
            smoothPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetSuperTrend(..)` with `ToSuperTrend(..)`", false)]
    public static IEnumerable<SuperTrendResult> GetSuperTrend<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 10, double multiplier = 3)
        where TQuote : IQuote
        => quotes.ToSortedList().ToSuperTrend(lookbackPeriods, multiplier);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetT3(..)` with `ToT3(..)`", false)]
    public static IEnumerable<T3Result> GetT3<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 5, double volumeFactor = 0.7)
        where TQuote : IQuote
        => quotes.ToSortedList().ToT3(lookbackPeriods, volumeFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetTema(..)` with `ToTema(..)`", false)]
    public static IEnumerable<TemaResult> GetTema<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().ToTema(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetTr(..)` with `ToTr(..)`", false)]
    public static IEnumerable<TrResult> GetTr<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes.ToSortedList().ToTr();

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetTrix(..)` with `ToTrix(..)`", false)]
    public static IEnumerable<TrixResult> GetTrix<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote => quotes.ToSortedList().ToTrix(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetTsi(..)` with `ToTsi(..)`", false)]
    public static IEnumerable<TsiResult> GetTsi<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        where TQuote : IQuote
        => quotes.ToSortedList()
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetUlcerIndex(..)` with `ToUlcerIndex(..)`", false)]
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 14)
        where TQuote : IQuote
        => quotes.ToSortedList().ToUlcerIndex(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetUltimate(..)` with `ToUltimate(..)`", false)]
    public static IEnumerable<UltimateResult> GetUltimate<TQuote>(
        this IEnumerable<TQuote> quotes,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
        where TQuote : IQuote
        => quotes.ToSortedList().ToUltimate(shortPeriods, middlePeriods, longPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetVolatilityStop(..)` with `ToVolatilityStop(..)`", false)]
    public static IEnumerable<VolatilityStopResult> GetVolatilityStop<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 7,
        double multiplier = 3)
        where TQuote : IQuote
        => quotes.ToSortedList().ToVolatilityStop(lookbackPeriods, multiplier);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetVortex(..)` with `ToVortex(..)`", false)]
    public static IEnumerable<VortexResult> GetVortex<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote => quotes.ToSortedList().ToVortex(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetVwap(..)` with `ToVwap(..)`", false)]
    public static IEnumerable<VwapResult> GetVwap<TQuote>(
        this IEnumerable<TQuote> quotes, DateTime? startDate = null)
        where TQuote : IQuote => quotes.ToSortedList().ToVwap(startDate);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetVwma(..)` with `ToVwma(..)`", false)]
    public static IEnumerable<VwmaResult> GetVwma<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote => quotes.ToSortedList().ToVwma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetWilliamsR(..)` with `ToWilliamsR(..)`", false)]
    public static IEnumerable<WilliamsResult> GetWilliamsR<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 14)
        where TQuote : IQuote => quotes.ToSortedList().ToWilliamsR(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetWma(..)` with `ToWma(..)`", false)]
    public static IEnumerable<WmaResult> GetWma<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote => quotes.ToSortedList().ToWma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetZigZag(..)` with `ToZigZag(..)`", false)]
    public static IEnumerable<ZigZagResult> GetZigZag<TQuote>(
        this IEnumerable<TQuote> quotes,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
        where TQuote : IQuote => quotes.ToSortedList().ToZigZag(endType, percentChange);

}
