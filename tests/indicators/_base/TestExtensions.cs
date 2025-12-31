namespace Skender.Stock.Indicators;

// Test-only extension methods for StreamHub creation from IReadOnlyList<IQuote>.
// These create standalone pre-populated hubs for testing convenience.
public static class TestExtensions
{
    public static AdlHub ToAdlHub(
        this IReadOnlyList<IQuote> quotes)
        => quotes.ToQuoteHub().ToAdlHub();

    public static AdxHub ToAdxHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAdxHub(lookbackPeriods);
    }

    public static AlligatorHub ToAlligatorHub(
        this IReadOnlyList<IQuote> quotes,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAlligatorHub(jawPeriods, jawOffset, teethPeriods, teethOffset, lipsPeriods, lipsOffset);
    }

    public static AlmaHub ToAlmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAlmaHub(lookbackPeriods, offset, sigma);
    }

    public static AroonHub ToAroonHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 25)
        => quotes.ToQuoteHub().ToAroonHub(lookbackPeriods);

    public static AtrHub ToAtrHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAtrHub(lookbackPeriods);
    }

    public static AtrStopHub ToAtrStopHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 21,
        double multiplier = 3,
        EndType endType = EndType.Close)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAtrStopHub(lookbackPeriods, multiplier, endType);
    }

    public static AwesomeHub ToAwesomeHub(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 5,
        int slowPeriods = 34)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAwesomeHub(fastPeriods, slowPeriods);
    }

    public static BollingerBandsHub ToBollingerBandsHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToBollingerBandsHub(lookbackPeriods, standardDeviations);
    }

    public static BopHub ToBopHub(
        this IReadOnlyList<IQuote> quotes,
        int smoothPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToBopHub(smoothPeriods);
    }

    public static CciHub ToCciHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCciHub(lookbackPeriods);
    }

    public static ChaikinOscHub ToChaikinOscHub(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 3,
        int slowPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToChaikinOscHub(fastPeriods, slowPeriods);
    }

    public static ChandelierHub ToChandelierHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToChandelierHub(lookbackPeriods, multiplier, type);
    }

    public static ChopHub ToChopHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToChopHub(lookbackPeriods);
    }

    public static CmfHub ToCmfHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCmfHub(lookbackPeriods);
    }

    public static CmoHub ToCmoHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCmoHub(lookbackPeriods);
    }

    public static ConnorsRsiHub ToConnorsRsiHub(
        this IReadOnlyList<IQuote> quotes,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);
    }

    public static DemaHub ToDemaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDemaHub(lookbackPeriods);
    }

    public static DojiHub ToDojiHub(
        this IReadOnlyList<IQuote> quotes,
        double maxPriceChangePercent = 0.1)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDojiHub(maxPriceChangePercent);
    }

    public static DonchianHub ToDonchianHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDonchianHub(lookbackPeriods);
    }

    public static DpoHub ToDpoHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDpoHub(lookbackPeriods);
    }

    public static DynamicHub ToDynamicHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods,
        double kFactor = 0.6)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDynamicHub(lookbackPeriods, kFactor);
    }

    public static ElderRayHub ToElderRayHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToElderRayHub(lookbackPeriods);
    }

    public static EmaHub ToEmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToEmaHub(lookbackPeriods);

        // reminder: can't be self-ref 'quotes.ToHub' syntax
    }

    public static EpmaHub ToEpmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToEpmaHub(lookbackPeriods);
    }

    public static FcbHub ToFcbHub(
        this IReadOnlyList<IQuote> quotes,
        int windowSpan = 2)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToFcbHub(windowSpan);
    }

    public static FisherTransformHub ToFisherTransformHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToFisherTransformHub(lookbackPeriods);
    }

    public static ForceIndexHub ToForceIndexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 2)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToForceIndexHub(lookbackPeriods);
    }

    public static FractalHub ToFractalHub(
        this IReadOnlyList<IQuote> quotes,
        int windowSpan = 2,
        EndType endType = EndType.HighLow)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToFractalHub(windowSpan, endType);
    }

    public static FractalHub ToFractalHub(
        this IReadOnlyList<IQuote> quotes,
        int leftSpan,
        int rightSpan,
        EndType endType = EndType.HighLow)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToFractalHub(leftSpan, rightSpan, endType);
    }

    public static GatorHub ToGatorHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToGatorHub();
    }

    public static HeikinAshiHub ToHeikinAshiHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHeikinAshiHub();
    }

    public static HmaHub ToHmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHmaHub(lookbackPeriods);
    }

    public static HtTrendlineHub ToHtTrendlineHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHtTrendlineHub();
    }

    public static HurstHub ToHurstHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 100)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHurstHub(lookbackPeriods);
    }

    public static IchimokuHub ToIchimokuHub(
        this IReadOnlyList<IQuote> quotes,
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToIchimokuHub(tenkanPeriods, kijunPeriods, senkouBPeriods);
    }

    public static IchimokuHub ToIchimokuHub(
        this IReadOnlyList<IQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int offsetPeriods)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToIchimokuHub(tenkanPeriods, kijunPeriods, senkouBPeriods, offsetPeriods);
    }

    public static KamaHub ToKamaHub(
        this IReadOnlyList<IQuote> quotes,
        int erPeriods = 10,
        int fastPeriods = 2,
        int slowPeriods = 30)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToKamaHub(erPeriods, fastPeriods, slowPeriods);
    }

    public static KeltnerHub ToKeltnerHub(
        this IReadOnlyList<IQuote> quotes,
        int emaPeriods = 20,
        double multiplier = 2,
        int atrPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToKeltnerHub(emaPeriods, multiplier, atrPeriods);
    }

    public static KvoHub ToKvoHub(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 34,
        int slowPeriods = 55,
        int signalPeriods = 13)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToKvoHub(fastPeriods, slowPeriods, signalPeriods);
    }

    public static MacdHub ToMacdHub(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMacdHub(fastPeriods, slowPeriods, signalPeriods);
    }

    public static MamaHub ToMamaHub(
        this IReadOnlyList<IQuote> quotes,
        double fastLimit = 0.5,
        double slowLimit = 0.05)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMamaHub(fastLimit, slowLimit);
    }

    public static MarubozuHub ToMarubozuHub(
        this IReadOnlyList<IQuote> quotes,
        double minBodyPercent = 95)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMarubozuHub(minBodyPercent);
    }

    public static MfiHub ToMfiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMfiHub(lookbackPeriods);
    }

    public static ObvHub ToObvHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToObvHub();
    }

    public static ParabolicSarHub ToParabolicSarHub(
        this IReadOnlyList<IQuote> quotes,
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToParabolicSarHub(accelerationStep, maxAccelerationFactor);
    }

    public static ParabolicSarHub ToParabolicSarHub(
        this IReadOnlyList<IQuote> quotes,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToParabolicSarHub(accelerationStep, maxAccelerationFactor, initialFactor);
    }

    public static PivotPointsHub ToPivotPointsHub(
        this IReadOnlyList<IQuote> quotes,
        PeriodSize windowSize = PeriodSize.Month,
        PivotPointType pointType = PivotPointType.Standard)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToPivotPointsHub(windowSize, pointType);
    }

    public static PivotsHub ToPivotsHub(
        this IReadOnlyList<IQuote> quotes,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToPivotsHub(leftSpan, rightSpan, maxTrendPeriods, endType);
    }

    public static PmoHub ToPmoHub(
        this IReadOnlyList<IQuote> quotes,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToPmoHub(timePeriods, smoothPeriods, signalPeriods);
    }

    public static PvoHub ToPvoHub(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToPvoHub(fastPeriods, slowPeriods, signalPeriods);
    }

    public static QuoteHub ToQuoteHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();  // cannot dogfood ToQuoteHub() here
        quoteHub.Add(quotes);
        return quoteHub;
    }

    public static QuotePartHub ToQuotePartHub(
        this IReadOnlyList<IQuote> quotes,
        CandlePart candlePart)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToQuotePartHub(candlePart);
    }

    public static RenkoHub ToRenkoHub(
        this IReadOnlyList<IQuote> quotes,
        decimal brickSize,
        EndType endType = EndType.Close)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRenkoHub(brickSize, endType);
    }

    public static RocHub ToRocHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRocHub(lookbackPeriods);
    }

    public static RocWbHub ToRocWbHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20,
        int emaPeriods = 5,
        int stdDevPeriods = 5)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRocWbHub(lookbackPeriods, emaPeriods, stdDevPeriods);
    }

    public static RollingPivotsHub ToRollingPivotsHub(
        this IReadOnlyList<IQuote> quotes,
        int windowPeriods = 20,
        int offsetPeriods = 0,
        PivotPointType pointType = PivotPointType.Standard)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRollingPivotsHub(windowPeriods, offsetPeriods, pointType);
    }

    public static RsiHub ToRsiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRsiHub(lookbackPeriods);
    }

    public static SlopeHub ToSlopeHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSlopeHub(lookbackPeriods);

        // reminder: can't be self-ref 'quotes.ToHub' syntax
    }

    public static SmaAnalysisHub ToSmaAnalysisHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmaAnalysisHub(lookbackPeriods);
    }

    public static SmaHub ToSmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmaHub(lookbackPeriods);
    }

    public static SmiHub ToSmiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13,
        int firstSmoothPeriods = 25,
        int secondSmoothPeriods = 2,
        int signalPeriods = 3)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmiHub(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);
    }

    public static SmmaHub ToSmmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmmaHub(lookbackPeriods);
    }

    public static StcHub ToStcHub(
        this IReadOnlyList<IQuote> quotes,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStcHub(cyclePeriods, fastPeriods, slowPeriods);
    }

    public static StarcBandsHub ToStarcBandsHub(
        this IReadOnlyList<IQuote> quotes,
        int smaPeriods = 5,
        double multiplier = 2,
        int atrPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStarcBandsHub(smaPeriods, multiplier, atrPeriods);
    }

    public static StdDevHub ToStdDevHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStdDevHub(lookbackPeriods);
    }

    public static StochHub ToStochHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 3)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStochHub(lookbackPeriods, signalPeriods, smoothPeriods);
    }

    public static StochRsiHub ToStochRsiHub(
        this IReadOnlyList<IQuote> quotes,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStochRsiHub(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);
    }

    public static SuperTrendHub ToSuperTrendHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 10,
        double multiplier = 3)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSuperTrendHub(lookbackPeriods, multiplier);
    }

    public static T3Hub ToT3Hub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToT3Hub(lookbackPeriods, volumeFactor);
    }

    public static TemaHub ToTemaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTemaHub(lookbackPeriods);
    }

    public static TrHub ToTrHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTrHub();
    }

    public static TrixHub ToTrixHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTrixHub(lookbackPeriods);
    }

    public static TsiHub ToTsiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);
    }

    public static UlcerIndexHub ToUlcerIndexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToUlcerIndexHub(lookbackPeriods);
    }

    public static UltimateHub ToUltimateHub(
        this IReadOnlyList<IQuote> quotes,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToUltimateHub(shortPeriods, middlePeriods, longPeriods);
    }

    public static VolatilityStopHub ToVolatilityStopHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 7,
        double multiplier = 3)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVolatilityStopHub(lookbackPeriods, multiplier);
    }

    public static VortexHub ToVortexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVortexHub(lookbackPeriods);
    }

    public static VwapHub ToVwapHub(
        this IReadOnlyList<IQuote> quotes,
        DateTime? startDate = null)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVwapHub(startDate);
    }

    public static VwmaHub ToVwmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVwmaHub(lookbackPeriods);
    }

    public static WilliamsRHub ToWilliamsRHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWilliamsRHub(lookbackPeriods);
    }

    public static WmaHub ToWmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWmaHub(lookbackPeriods);
    }
}
