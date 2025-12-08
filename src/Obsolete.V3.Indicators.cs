using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;
#pragma warning disable CS1591, IND001, RCS1163

// OBSOLETE IN v3.0.0
public static partial class Indicator
{
    // GENERAL INDICATOR METHODS
    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetAdl(..)` to `ToAdl(..)`", false)]
    public static IEnumerable<AdlResult> GetAdl(
    this IEnumerable<IQuote> quotes)
    => quotes.ToSortedList().ToAdl();

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.ToSma(smaPeriods)` for moving averages.", true)]
    public static IEnumerable<AdlResult> GetAdl(
    this IEnumerable<IQuote> quotes, int smaPeriods)
    => quotes.ToSortedList().ToAdl();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetAdx(..)` to `ToAdx(..)`", false)]
    public static IEnumerable<AdxResult> GetAdx(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 14)
        => quotes.ToSortedList().ToAdx(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetAlligator(..)` to `ToAlligator(..)`", false)]
    public static IEnumerable<AlligatorResult> GetAlligator(
        this IEnumerable<IQuote> quotes,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        => quotes.ToSortedList().ToAlligator(
            jawPeriods, jawOffset,
            teethPeriods, teethOffset,
            lipsPeriods, lipsOffset);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToAlligator(..)' method.  Tuple arguments were removed.", false)]
    public static IEnumerable<AlligatorResult> GetAlligator(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        => priceTuples
           .Select(static t => new QuotePart(t.d, t.v))
           .ToSortedList()
           .ToAlligator(
            jawPeriods, jawOffset,
            teethPeriods, teethOffset,
            lipsPeriods, lipsOffset);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetAlma(..)` to `ToAlma(..)`", false)]
    public static IEnumerable<AlmaResult> GetAlma(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        => quotes.ToSortedList().ToAlma(lookbackPeriods, offset, sigma);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToAlma(..)' method.  Tuple arguments were removed.", false)]
    public static IEnumerable<AlmaResult> GetAlma(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6) => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToAlma(lookbackPeriods, offset, sigma);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetAroon(..)` to `ToAroon(..)`", false)]
    public static IEnumerable<AroonResult> GetAroon(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 25)
        => quotes.ToSortedList().ToAroon(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetAtr(..)` to `ToAtr(..)`", false)]
    public static IEnumerable<AtrResult> GetAtr(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 14)
        => quotes.ToSortedList().ToAtr(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetAtrStop(..)` to `ToAtrStop(..)`", false)]
    public static IEnumerable<AtrStopResult> GetAtrStop(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods = 21,
        double multiplier = 3,
        EndType endType = EndType.Close)
        => quotes.ToSortedList().ToAtrStop(lookbackPeriods, multiplier, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetAwesome(..)` to `ToAwesome(..)`", false)]
    public static IEnumerable<AwesomeResult> GetAwesome(
        this IEnumerable<IQuote> quotes, int fastPeriods = 5, int slowPeriods = 34)
        => quotes.ToSortedList().ToAwesome(fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToAwesome(..)' method.  Tuple arguments were removed.", false)]
    public static IEnumerable<AwesomeResult> GetAwesome(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int fastPeriods = 5,
        int slowPeriods = 34)
        => priceTuples
           .Select(static t => new QuotePart(t.d, t.v))
           .ToSortedList()
           .ToAwesome(fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetBeta(..)` to `ToBeta(..)`", false)]
    public static IEnumerable<BetaResult> GetBeta(
        this IEnumerable<IQuote> quotesEval,
        IEnumerable<IQuote> quotesMarket,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        => quotesEval
            .ToSortedReusableList()
            .ToBeta(quotesMarket.ToSortedReusableList(), lookbackPeriods, type);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToBeta(..)' method.  Tuple arguments were removed.", false)]
    public static IEnumerable<BetaResult> GetBeta(
        this IEnumerable<(DateTime d, double v)> evalTuple,
        IEnumerable<(DateTime d, double v)> mrktTuple,
        int lookbackPeriods,
        BetaType type = BetaType.Standard)
        => evalTuple
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToBeta(
                mrktTuple
                    .Select(static t => new QuotePart(t.d, t.v))
                    .ToSortedList(),
                lookbackPeriods,
                type);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetBollingerBands(..)` to `ToBollingerBands(..)`", false)]
    public static IEnumerable<BollingerBandsResult> GetBollingerBands(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        => quotes.ToSortedList()
            .ToBollingerBands(
                lookbackPeriods,
                standardDeviations);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToBollingerBands(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<BollingerBandsResult> GetBollingerBands(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToBollingerBands(lookbackPeriods, standardDeviations);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetBop(..)` to `ToBop(..)`", false)]
    public static IEnumerable<BopResult> GetBop(
        this IEnumerable<IQuote> quotes, int smoothPeriods = 14)
        => quotes.ToSortedList().ToBop(smoothPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetCci(..)` to `ToCci(..)`", false)]
    public static IEnumerable<CciResult> GetCci(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 20)
        => quotes.ToSortedList().ToCci(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetChaikinOsc(..)` to `ToChaikinOsc(..)`", false)]
    public static IEnumerable<ChaikinOscResult> GetChaikinOsc(
        this IEnumerable<IQuote> quotes, int fastPeriods = 3, int slowPeriods = 10)
        => quotes.ToSortedList().ToChaikinOsc(fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetChandelier(..)` to `ToChandelier(..)`", false)]
    public static IEnumerable<ChandelierResult> GetChandelier(
        this IEnumerable<IQuote> quotes,
            int lookbackPeriods = 22,
            double multiplier = 3,
            ChandelierType type = ChandelierType.Long)
        => quotes.ToSortedList()
            .ToChandelier(lookbackPeriods, multiplier, (Direction)type);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetChop(..)` to `ToChop(..)`", false)]
    public static IEnumerable<ChopResult> GetChop(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 14)
        => quotes.ToSortedList().ToChop(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetCmf(..)` to `ToCmf(..)`", false)]
    public static IEnumerable<CmfResult> GetCmf(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 20)
        => quotes.ToSortedList().ToCmf(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetCmo(..)` to `ToCmo(..)`", false)]
    public static IEnumerable<CmoResult> GetCmo(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToCmo(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToCmo(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<CmoResult> GetCmo(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToCmo(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetConnorsRsi(..)` to `ToConnorsRsi(..)`", false)]
    public static IEnumerable<ConnorsRsiResult> GetConnorsRsi(
        this IEnumerable<IQuote> quotes,
        int rsiPeriods = 3, int streakPeriods = 2, int rankPeriods = 100)
        => quotes.ToSortedList()
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToConnorsRsi(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<ConnorsRsiResult> GetConnorsRsi(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetCorrelation(..)` to `ToCorrelation(..)`", false)]
    public static IEnumerable<CorrResult> GetCorrelation(
        this IEnumerable<IQuote> quotesA,
        IEnumerable<IQuote> quotesB, int lookbackPeriods)
        => quotesA.ToSortedList().ToCorrelation(quotesB.ToSortedList(), lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToCorrelation(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<CorrResult> GetCorrelation(
        this IEnumerable<(DateTime d, double v)> tuplesA,
        IEnumerable<(DateTime d, double v)> tuplesB,
        int lookbackPeriods)
        => tuplesA
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToCorrelation(
                tuplesB
                    .Select(static t => new QuotePart(t.d, t.v))
                    .ToSortedList(),
                lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetDema(..)` to `ToDema(..)`", false)]
    public static IEnumerable<DemaResult> GetDema(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToDema(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToDema(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<DemaResult> GetDema(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToDema(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetDoji(..)` to `ToDoji(..)`", false)]
    public static IEnumerable<CandleResult> GetDoji(
        this IEnumerable<IQuote> quotes, double maxPriceChangePercent = 0.1)
        => quotes.ToSortedList().ToDoji(maxPriceChangePercent);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetDonchian(..)` to `ToDonchian(..)`", false)]
    public static IEnumerable<DonchianResult> GetDonchian(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 20)
        => quotes.ToSortedList().ToDonchian(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetDpo(..)` to `ToDpo(..)`", false)]
    public static IEnumerable<DpoResult> GetDpo(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToDpo(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToDpo(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<DpoResult> GetDpo(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToDpo(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetDynamic(..)` to `ToDynamic(..)`", false)]
    public static IEnumerable<DynamicResult> GetDynamic(
        this IEnumerable<IQuote> quotes, int lookbackPeriods, double kFactor = 0.6)
        => quotes.ToSortedList().ToDynamic(lookbackPeriods, kFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToDynamic(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<DynamicResult> GetDynamic(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods,
        double kFactor = 0.6)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToDynamic(lookbackPeriods, kFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetElderRay(..)` to `ToElderRay(..)`", false)]
    public static IEnumerable<ElderRayResult> GetElderRay(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 13)
        => quotes.ToSortedList().ToElderRay(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetEma(..)` to `ToEma(..)`", false)]
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToEma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToEma(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<EmaResult> GetEma(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToEma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetEpma(..)` to `ToEpma(..)`", false)]
    public static IEnumerable<EpmaResult> GetEpma(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToEpma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToEpma(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<EpmaResult> GetEpma(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToEpma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetFcb(..)` to `ToFcb(..)`", false)]
    public static IEnumerable<FcbResult> GetFcb(
        this IEnumerable<IQuote> quotes, int windowSpan = 2)
        => quotes.ToSortedList().ToFcb(windowSpan);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetFisherTransform(..)` to `ToFisherTransform(..)`", false)]
    public static IEnumerable<FisherTransformResult> GetFisherTransform(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 10)
        => quotes.ToSortedList().ToFisherTransform(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToFisherTransform(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<FisherTransformResult> GetFisherTransform(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 10)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToFisherTransform(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetForceIndex(..)` to `ToForceIndex(..)`", false)]
    public static IEnumerable<ForceIndexResult> GetForceIndex(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 2)
        => quotes.ToSortedList().ToForceIndex(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetFractal(..)` to `ToFractal(..)`", false)]
    public static IEnumerable<FractalResult> GetFractal(
        this IEnumerable<IQuote> quotes, int windowSpan = 2, EndType endType = EndType.HighLow)
        => quotes.ToSortedList().ToFractal(windowSpan, windowSpan, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetFractal(..)` to `ToFractal(..)`", false)]
    public static IEnumerable<FractalResult> GetFractal(
        this IEnumerable<IQuote> quotes, int leftSpan, int rightSpan, EndType endType = EndType.HighLow)
        => quotes.ToSortedList().ToFractal(leftSpan, rightSpan, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetGator(..)` to `ToGator(..)`", false)]
    public static IEnumerable<GatorResult> GetGator(
        this IEnumerable<IQuote> quotes)
        => quotes.ToSortedList().ToGator();

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToGator(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<GatorResult> GetGator(
        this IEnumerable<(DateTime d, double v)> priceTuples)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToGator();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetHeikinAshi(..)` to `ToHeikinAshi(..)`", false)]
    public static IEnumerable<HeikinAshiResult> GetHeikinAshi(
        this IEnumerable<IQuote> quotes)
        => quotes.ToSortedList().ToHeikinAshi();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetHma(..)` to `ToHma(..)`", false)]
    public static IEnumerable<HmaResult> GetHma(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToHma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToHma(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<HmaResult> GetHma(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToHma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetHtTrendline(..)` to `ToHtTrendline(..)`", false)]
    public static IEnumerable<HtlResult> GetHtTrendline(
        this IEnumerable<IQuote> quotes)
        => quotes.ToSortedList().ToHtTrendline();

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToHtTrendline(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<HtlResult> GetHtTrendline(
        this IEnumerable<(DateTime d, double v)> priceTuples)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToHtTrendline();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetHurst(..)` to `ToHurst(..)`", false)]
    public static IEnumerable<HurstResult> GetHurst<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods = 100)
        where TQuote : IQuote
        => quotes.ToSortedReusableList().ToHurst(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToHurst(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<HurstResult> GetHurst(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 100)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToHurst(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetIchimoku(..)` to `ToIchimoku(..)`", false)]
    public static IEnumerable<IchimokuResult> GetIchimoku(
        this IEnumerable<IQuote> quotes,
            int tenkanPeriods = 9,
            int kijunPeriods = 26,
            int senkouBPeriods = 52)
        => quotes.ToSortedList()
            .ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetKama(..)` to `ToKama(..)`", false)]
    public static IEnumerable<KamaResult> GetKama(
        this IEnumerable<IQuote> quotes,
            int erPeriods = 10,
            int fastPeriods = 2,
            int slowPeriods = 30)
        => quotes.ToSortedList().ToKama(erPeriods, fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToKama(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<KamaResult> GetKama(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToKama(erPeriods, fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetKeltner(..)` to `ToKeltner(..)`", false)]
    public static IEnumerable<KeltnerResult> GetKeltner(
        this IEnumerable<IQuote> quotes,
        int emaPeriods = 20,
        int multiplier = 2,
        int atrPeriods = 10)
        => quotes.ToSortedList().ToKeltner(emaPeriods, multiplier, atrPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetKvo(..)` to `ToKvo(..)`", false)]
    public static IEnumerable<KvoResult> GetKvo(
        this IEnumerable<IQuote> quotes,
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
        => quotes.ToSortedList()
        .ToKvo(fastPeriods, slowPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetMacd(..)` to `ToMacd(..)`", false)]
    public static IEnumerable<MacdResult> GetMacd(
        this IEnumerable<IQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        => quotes.ToSortedList().ToMacd(fastPeriods, slowPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToMacd(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<MacdResult> GetMacd(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToMacd(fastPeriods, slowPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetMaEnvelopes(..)` to `ToMaEnvelopes(..)`", false)]
    public static IEnumerable<MaEnvelopeResult> GetMaEnvelopes(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
        => quotes.ToSortedList()
            .ToMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToMaEnvelopes(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<MaEnvelopeResult> GetMaEnvelopes(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 20,
        double percentOffset = 2.5,
        MaType movingAverageType = MaType.SMA)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToMaEnvelopes(lookbackPeriods, percentOffset, movingAverageType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetMama(..)` to `ToMama(..)`", false)]
    public static IEnumerable<MamaResult> GetMama(
        this IEnumerable<IQuote> quotes,
        double fastLimit = 0.5,
        double slowLimit = 0.05)
        => quotes.ToSortedList().ToMama(fastLimit, slowLimit);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToMama(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<MamaResult> GetMama(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        double fastLimit = 0.5,
        double slowLimit = 0.05)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToMama(fastLimit, slowLimit);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetMarubozu(..)` to `ToMarubozu(..)`", false)]
    public static IEnumerable<CandleResult> GetMarubozu(
        this IEnumerable<IQuote> quotes, double minBodyPercent = 95)
        => quotes.ToSortedList().ToMarubozu(minBodyPercent);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetMfi(..)` to `ToMfi(..)`", false)]
    public static IEnumerable<MfiResult> GetMfi(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 14)
        => quotes.ToSortedList().ToMfi(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetObv(..)` to `ToObv(..)`", false)]
    public static IEnumerable<ObvResult> GetObv(
        this IEnumerable<IQuote> quotes)
        => quotes.ToSortedList().ToObv();

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.ToSma(smaPeriods)` for moving averages.", true)]
    public static IEnumerable<ObvResult> GetObv(
        this IEnumerable<IQuote> quotes, int smaPeriods)
        => quotes.ToSortedList().ToObv();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetParabolicSar(..)` to `ToParabolicSar(..)`", false)]
    public static IEnumerable<ParabolicSarResult> GetParabolicSar(
        this IEnumerable<IQuote> quotes,
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
        => quotes.ToSortedList()
            .ToParabolicSar(accelerationStep, maxAccelerationFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetParabolicSar(..)` to `ToParabolicSar(..)`", false)]
    public static IEnumerable<ParabolicSarResult> GetParabolicSar(
        this IEnumerable<IQuote> quotes,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
        => quotes.ToSortedList()
            .ToParabolicSar(accelerationStep, maxAccelerationFactor, initialFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetPivotPoints(..)` to `ToPivotPoints(..)`", false)]
    public static IEnumerable<PivotPointsResult> GetPivotPoints(
        this IEnumerable<IQuote> quotes,
        PeriodSize windowSize,
        PivotPointType pointType = PivotPointType.Standard)
        => quotes.ToSortedList().ToPivotPoints(windowSize, pointType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetPivots(..)` to `ToPivots(..)`", false)]
    public static IEnumerable<PivotsResult> GetPivots(
        this IEnumerable<IQuote> quotes,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
        => quotes.ToSortedList()
            .ToPivots(leftSpan, rightSpan, maxTrendPeriods, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetPmo(..)` to `ToPmo(..)`", false)]
    public static IEnumerable<PmoResult> GetPmo(
        this IEnumerable<IQuote> quotes,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
        => quotes.ToSortedList().ToPmo(timePeriods, smoothPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToPmo(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<PmoResult> GetPmo(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToPmo(timePeriods, smoothPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetPrs(..)` to `ToPrs(..)`", false)]
    public static IEnumerable<PrsResult> GetPrs(
    this IEnumerable<IQuote> quotesEval,
    IEnumerable<IQuote> quotesBase, int? lookbackPeriods = null)
    => quotesBase.ToSortedList()
        .ToPrs(quotesEval.ToSortedList(), lookbackPeriods ?? 0);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.ToSma(smaPeriods)` for moving averages.", true)]
    public static IEnumerable<PrsResult> GetPrs(
        this IEnumerable<IQuote> quotesEval,
        IEnumerable<IQuote> quotesBase, int? lookbackPeriods, int? smaPeriods = null)
        => quotesEval
            .ToSortedList()
            .Use(CandlePart.Close)
            .ToPrs(quotesBase.ToSortedList().Use(CandlePart.Close), lookbackPeriods ?? 0);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToPrs(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<PrsResult> GetPrs(
        this IEnumerable<(DateTime d, double v)> tupleEval,
        IEnumerable<(DateTime d, double v)> tupleBase,
        int lookbackPeriods = 0,
        int smaPeriods = 0)
        => tupleEval
            .Select(static t => new QuotePart(t.d, t.v)).ToSortedList()
            .ToPrs(tupleBase
                .Select(static t => new QuotePart(t.d, t.v)).ToSortedList(),
            lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetPvo(..)` to `ToPvo(..)`", false)]
    public static IEnumerable<PvoResult> GetPvo(
        this IEnumerable<IQuote> quotes,
        int fastPeriods = 9, int slowPeriods = 12, int signalPeriods = 9)
        => quotes.ToSortedList().ToPvo(fastPeriods, slowPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetRenko(..)` to `ToRenko(..)`", false)]
    public static IEnumerable<RenkoResult> GetRenko(
        this IEnumerable<IQuote> quotes, decimal brickSize, EndType endType = EndType.Close)
        => quotes.ToSortedList().ToRenko(brickSize, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetRenkoAtr(..)` to `ToRenkoAtr(..)`", false)]
    public static IEnumerable<RenkoResult> GetRenkoAtr(
        this IEnumerable<IQuote> quotes, int atrPeriods, EndType endType = EndType.Close)
        => quotes.ToSortedList().ToRenkoAtr(atrPeriods, endType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetRoc(..)` to `ToRoc(..)`", false)]
    public static IEnumerable<RocResult> GetRoc(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToRoc(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.ToSma(smaPeriods)` for moving averages.", true)]
    public static IEnumerable<RocResult> GetRoc(
        this IEnumerable<IQuote> quotes, int lookbackPeriods, int smaPeriods)
        => quotes.ToSortedList().Use(CandlePart.Close).ToRoc(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToRoc(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<RocResult> GetRoc(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods,
        int? smaPeriods = null)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToRoc(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetRocWb(..)` to `ToRocWb(..)`", false)]
    public static IEnumerable<RocWbResult> GetRocWb(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods, int emaPeriods, int stdDevPeriods)
        => quotes.ToSortedList()
            .ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToRocWb(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<RocWbResult> GetRocWb(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods,
        int emaPeriods,
        int stdDevPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToRocWb(lookbackPeriods, emaPeriods, stdDevPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetRollingPivots(..)` to `ToRollingPivots(..)`", false)]
    public static IEnumerable<RollingPivotsResult> GetRollingPivots(
        this IEnumerable<IQuote> quotes,
        int windowPeriods,
        int offsetPeriods,
        PivotPointType pointType = PivotPointType.Standard)
        => quotes.ToSortedList()
            .ToRollingPivots(windowPeriods, offsetPeriods, pointType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetRsi(..)` to `ToRsi(..)`", false)]
    public static IEnumerable<RsiResult> GetRsi(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 14)
        => quotes.ToSortedList().ToRsi(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToRsi(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<RsiResult> GetRsi(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToRsi(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetSlope(..)` to `ToSlope(..)`", false)]
    public static IEnumerable<SlopeResult> GetSlope(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToSlope(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToSlope(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<SlopeResult> GetSlope(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToSlope(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetSma(..)` to `ToSma(..)`", false)]
    public static IEnumerable<SmaResult> GetSma(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToSma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToSma(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<SmaResult> GetSma(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToSma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetSmaAnalysis(..)` to `ToSmaAnalysis(..)`", false)]
    public static IEnumerable<SmaAnalysisResult> GetSmaAnalysis(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToSmaAnalysis(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToSmaAnalysis(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<SmaAnalysisResult> GetSmaAnalysis(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToSmaAnalysis(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetSmi(..)` to `ToSmi(..)`", false)]
    public static IEnumerable<SmiResult> GetSmi(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
        => quotes.ToSortedList()
            .ToSmi(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetSmma(..)` to `ToSmma(..)`", false)]
    public static IEnumerable<SmmaResult> GetSmma(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToSmma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToSmma(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<SmmaResult> GetSmma(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToSmma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetStarcBands(..)` to `ToStarcBands(..)`", false)]
    public static IEnumerable<StarcBandsResult> GetStarcBands(
        this IEnumerable<IQuote> quotes,
        int smaPeriods,
        double multiplier = 2,
        int atrPeriods = 10)
        => quotes.ToSortedList().ToStarcBands(smaPeriods, multiplier, atrPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetStc(..)` to `ToStc(..)`", false)]
    public static IEnumerable<StcResult> GetStc(
        this IEnumerable<IQuote> quotes,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        => quotes.ToSortedList().ToStc(cyclePeriods, fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToStc(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<StcResult> GetStc(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToStc(cyclePeriods, fastPeriods, slowPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetStdDev(..)` to `ToStdDev(..)`", false)]
    public static IEnumerable<StdDevResult> GetStdDev(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToStdDev(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.ToSma(smaPeriods)` for moving averages.", true)]
    public static IEnumerable<StdDevResult> GetStdDev(
        this IEnumerable<IQuote> quotes, int lookbackPeriods, int smaPeriods)
        => quotes.ToSortedList().Use(CandlePart.Close).ToStdDev(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToStdDev(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<StdDevResult> GetStdDev(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToStdDev(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetStdDevChannels(..)` to `ToStdDevChannels(..)`. "
            + "If using `lookbackPeriods=null`, replace with `lookbackPeriods=source.Count`.", false)]
    public static IEnumerable<StdDevChannelsResult> GetStdDevChannels(
        this IEnumerable<IQuote> quotes, int? lookbackPeriods = 20, double stdDeviations = 2)
        => quotes.ToSortedList().ToStdDevChannels(lookbackPeriods ?? quotes.Count(), stdDeviations);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToStdDevChannels(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<StdDevChannelsResult> GetStdDevChannels(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 20,
        double stdDeviations = 2)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToStdDevChannels(lookbackPeriods, stdDeviations);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetStoch(..)` to `ToStoch(..)`", false)]
    public static IEnumerable<StochResult> GetStoch(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
        => quotes.ToSortedList().ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToStochRsi(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<StochRsiResult> GetStochRsi(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetStoch(..)` to `ToStoch(..)`", false)]
    public static IEnumerable<StochResult> GetStoch(
            this IEnumerable<IQuote> quotes,
            int lookbackPeriods,
            int signalPeriods,
            int smoothPeriods,
            double kFactor,
            double dFactor,
            MaType movingAverageType)
        => quotes.ToSortedList().ToStoch(
            lookbackPeriods,
            signalPeriods,
            smoothPeriods,
            kFactor,
            dFactor,
            movingAverageType);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetStochRsi(..)` to `ToStochRsi(..)`", false)]
    public static IEnumerable<StochRsiResult> GetStochRsi(
            this IEnumerable<IQuote> quotes,
            int rsiPeriods,
            int stochPeriods,
            int signalPeriods,
            int smoothPeriods = 1)
        => quotes.ToSortedList().ToStochRsi(
            rsiPeriods,
            stochPeriods,
            signalPeriods,
            smoothPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetSuperTrend(..)` to `ToSuperTrend(..)`", false)]
    public static IEnumerable<SuperTrendResult> GetSuperTrend(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 10, double multiplier = 3)
        => quotes.ToSortedList().ToSuperTrend(lookbackPeriods, multiplier);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetT3(..)` to `ToT3(..)`", false)]
    public static IEnumerable<T3Result> GetT3(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 5, double volumeFactor = 0.7)
        => quotes.ToSortedList().ToT3(lookbackPeriods, volumeFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToT3(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<T3Result> GetT3(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToT3(lookbackPeriods, volumeFactor);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetTema(..)` to `ToTema(..)`", false)]
    public static IEnumerable<TemaResult> GetTema(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToTema(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToTema(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<TemaResult> GetTema(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToTema(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetTr(..)` to `ToTr(..)`", false)]
    public static IEnumerable<TrResult> GetTr(
        this IEnumerable<IQuote> quotes)
        => quotes.ToSortedList().ToTr();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetTrix(..)` to `ToTrix(..)`", false)]
    public static IEnumerable<TrixResult> GetTrix(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToTrix(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToTrix(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<TrixResult> GetTrix(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToTrix(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.ToSma(smaPeriods)` for moving averages.", true)]
    public static IEnumerable<TrixResult> GetTrix(
        this IEnumerable<IQuote> quotes, int lookbackPeriods, int signalPeriods)
        => quotes.ToSortedList().Use(CandlePart.Close).ToTrix(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetTsi(..)` to `ToTsi(..)`", false)]
    public static IEnumerable<TsiResult> GetTsi(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        => quotes.ToSortedList()
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToTsi(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<TsiResult> GetTsi(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToTsi(lookbackPeriods, smoothPeriods, signalPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetUlcerIndex(..)` to `ToUlcerIndex(..)`", false)]
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 14)
        => quotes.ToSortedList().ToUlcerIndex(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToUlcerIndex(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<UlcerIndexResult> GetUlcerIndex(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods = 14)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToUlcerIndex(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetUltimate(..)` to `ToUltimate(..)`", false)]
    public static IEnumerable<UltimateResult> GetUltimate(
        this IEnumerable<IQuote> quotes,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
        => quotes.ToSortedList().ToUltimate(shortPeriods, middlePeriods, longPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetVolatilityStop(..)` to `ToVolatilityStop(..)`", false)]
    public static IEnumerable<VolatilityStopResult> GetVolatilityStop(
        this IEnumerable<IQuote> quotes,
        int lookbackPeriods = 7,
        double multiplier = 3)
        => quotes.ToSortedList().ToVolatilityStop(lookbackPeriods, multiplier);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetVortex(..)` to `ToVortex(..)`", false)]
    public static IEnumerable<VortexResult> GetVortex(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToVortex(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetVwap(..)` to `ToVwap(..)`", false)]
    public static IEnumerable<VwapResult> GetVwap(
        this IEnumerable<IQuote> quotes, DateTime? startDate = null)
    {
        IReadOnlyList<IQuote> source = quotes.ToSortedList();

        return source?.Count is null or 0
            ? []
            : source.ToVwap(startDate ?? source[0].Timestamp);
    }

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetVwma(..)` to `ToVwma(..)`", false)]
    public static IEnumerable<VwmaResult> GetVwma(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToVwma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetWilliamsR(..)` to `ToWilliamsR(..)`", false)]
    public static IEnumerable<WilliamsResult> GetWilliamsR(
        this IEnumerable<IQuote> quotes, int lookbackPeriods = 14)
        => quotes.ToSortedList().ToWilliamsR(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetWma(..)` to `ToWma(..)`", false)]
    public static IEnumerable<WmaResult> GetWma(
        this IEnumerable<IQuote> quotes, int lookbackPeriods)
        => quotes.ToSortedList().ToWma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use 'ToWma(..)' method. Tuple arguments were removed.", false)]
    public static IEnumerable<WmaResult> GetWma(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int lookbackPeriods)
        => priceTuples
            .Select(static t => new QuotePart(t.d, t.v))
            .ToSortedList()
            .ToWma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename `GetZigZag(..)` to `ToZigZag(..)`", false)]
    public static IEnumerable<ZigZagResult> GetZigZag(
        this IEnumerable<IQuote> quotes,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
        => quotes.ToSortedList().ToZigZag(endType, percentChange);
}
