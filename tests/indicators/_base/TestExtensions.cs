namespace Skender.Stock.Indicators;

/// <summary>
/// Test-only extension methods for StreamHub creation from IReadOnlyList&lt;IQuote&gt;.
/// These create standalone pre-populated hubs for testing convenience.
/// Production code should use QuoteHub with ToXxxHub() for real streaming.
/// </summary>
public static class TestExtensions
{

    /// <summary>
    /// Creates a standalone AdlHub from an initiating collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public static AdlHub ToAdlHub(
        this IReadOnlyList<IQuote> quotes)
        => quotes.ToQuoteHub().ToAdlHub();


    /// <summary>
    /// Creates an ADX hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="AdxHub"/>.</returns>
    public static AdxHub ToAdxHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAdxHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates an Alligator hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="jawPeriods">The number of periods for the jaw.</param>
    /// <param name="jawOffset">The offset for the jaw.</param>
    /// <param name="teethPeriods">The number of periods for the teeth.</param>
    /// <param name="teethOffset">The offset for the teeth.</param>
    /// <param name="lipsPeriods">The number of periods for the lips.</param>
    /// <param name="lipsOffset">The offset for the lips.</param>
    /// <returns>An instance of <see cref="AlligatorHub"/>.</returns>
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


    /// <summary>
    /// Creates an ALMA hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="offset">The offset for the ALMA calculation. Default is 0.85.</param>
    /// <param name="sigma">The sigma for the ALMA calculation. Default is 6.</param>
    /// <returns>An instance of <see cref="AlmaHub"/>.</returns>
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


    /// <summary>
    /// Creates an Aroon hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="AroonHub"/>.</returns>
    public static AroonHub ToAroonHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 25)
        => quotes.ToQuoteHub().ToAroonHub(lookbackPeriods);


    /// <summary>
    /// Creates a Atr hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="AtrHub"/>.</returns>
    public static AtrHub ToAtrHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAtrHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a AtrStop hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Parameter for the calculation.</param>
    /// <param name="endType">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="AtrStopHub"/>.</returns>
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


    /// <summary>
    /// Creates an Awesome hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastPeriods">The number of periods for the fast moving average.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average.</param>
    /// <returns>An instance of <see cref="AwesomeHub"/>.</returns>
    public static AwesomeHub ToAwesomeHub(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 5,
        int slowPeriods = 34)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAwesomeHub(fastPeriods, slowPeriods);
    }


    /// <summary>
    /// Creates a BollingerBands hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="BollingerBandsHub"/>.</returns>
    public static BollingerBandsHub ToBollingerBandsHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToBollingerBandsHub(lookbackPeriods, standardDeviations);
    }


    /// <summary>
    /// Creates a Bop hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="smoothPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="BopHub"/>.</returns>
    public static BopHub ToBopHub(
        this IReadOnlyList<IQuote> quotes,
        int smoothPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToBopHub(smoothPeriods);
    }


    /// <summary>
    /// Creates a CCI hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="CciHub"/>.</returns>
    public static CciHub ToCciHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCciHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a ChaikinOsc hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="ChaikinOscHub"/>.</returns>
    public static ChaikinOscHub ToChaikinOscHub(
        this IReadOnlyList<IQuote> quotes,
        int fastPeriods = 3,
        int slowPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToChaikinOscHub(fastPeriods, slowPeriods);
    }


    /// <summary>
    /// Creates a Chandelier Exit hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    /// <returns>An instance of <see cref="ChandelierHub"/>.</returns>
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


    /// <summary>
    /// Creates a Chop hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="ChopHub"/>.</returns>
    public static ChopHub ToChopHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToChopHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Cmf hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="CmfHub"/>.</returns>
    public static CmfHub ToCmfHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCmfHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Cmo hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="CmoHub"/>.</returns>
    public static CmoHub ToCmoHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCmoHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a ConnorsRsi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="rsiPeriods">The number of periods to use for the RSI calculation. Default is 3.</param>
    /// <param name="streakPeriods">The number of periods to use for the streak calculation. Default is 2.</param>
    /// <param name="rankPeriods">The number of periods to use for the percent rank calculation. Default is 100.</param>
    /// <returns>An instance of <see cref="ConnorsRsiHub"/>.</returns>
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


    /// <summary>
    /// Creates a Dema hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="DemaHub"/>.</returns>
    public static DemaHub ToDemaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDemaHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Doji hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="maxPriceChangePercent">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="DojiHub"/>.</returns>
    public static DojiHub ToDojiHub(
        this IReadOnlyList<IQuote> quotes,
        double maxPriceChangePercent = 0.1)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDojiHub(maxPriceChangePercent);
    }


    /// <summary>
    /// Creates a DPO hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="DpoHub"/>.</returns>
    public static DpoHub ToDpoHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDpoHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Dynamic hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="kFactor">The smoothing factor for the calculation.</param>
    /// <returns>An instance of <see cref="DynamicHub"/>.</returns>
    public static DynamicHub ToDynamicHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods,
        double kFactor = 0.6)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDynamicHub(lookbackPeriods, kFactor);
    }


    /// <summary>
    /// Creates an Elder Ray hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 13.</param>
    /// <returns>An instance of <see cref="ElderRayHub"/>.</returns>
    public static ElderRayHub ToElderRayHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToElderRayHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a standalone EMA hub with an internal <see cref="IQuote"/> cache
    /// based on an initial set of provided quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A standalone instance of <see cref="EmaHub"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="quotes"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public static EmaHub ToEmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToEmaHub(lookbackPeriods);

        // reminder: can't be self-ref 'quotes.ToHub' syntax
    }


    /// <summary>
    /// Creates a Epma hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="EpmaHub"/>.</returns>
    public static EpmaHub ToEpmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToEpmaHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Fcb hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="windowSpan">The window span for the calculation. Default is 2.</param>
    /// <returns>An instance of <see cref="FcbHub"/>.</returns>
    public static FcbHub ToFcbHub(
        this IReadOnlyList<IQuote> quotes,
        int windowSpan = 2)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToFcbHub(windowSpan);
    }


    /// <summary>
    /// Creates a Fisher Transform hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation. Default is 10.</param>
    /// <returns>An instance of <see cref="FisherTransformHub"/>.</returns>
    public static FisherTransformHub ToFisherTransformHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToFisherTransformHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Force Index hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="ForceIndexHub"/>.</returns>
    public static ForceIndexHub ToForceIndexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 2)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToForceIndexHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Fractal hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="windowSpan">Parameter for the calculation.</param>
    /// <param name="endType">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="FractalHub"/>.</returns>
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

    /// <summary>
    /// Creates a Fractal hub from a collection of quotes with different left and right spans.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="leftSpan">Parameter for the calculation.</param>
    /// <param name="rightSpan">Parameter for the calculation.</param>
    /// <param name="endType">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="FractalHub"/>.</returns>
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


    /// <summary>
    /// Creates a Gator hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>An instance of <see cref="GatorHub"/>.</returns>
    public static GatorHub ToGatorHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToGatorHub();
    }


    /// <summary>
    /// Creates a Heikin-Ashi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>An instance of <see cref="HeikinAshiHub"/>.</returns>
    public static HeikinAshiHub ToHeikinAshiHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHeikinAshiHub();
    }


    /// <summary>
    /// Creates a Hma hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="HmaHub"/>.</returns>
    public static HmaHub ToHmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHmaHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates an HtTrendline hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>An instance of <see cref="HtTrendlineHub"/>.</returns>
    public static HtTrendlineHub ToHtTrendlineHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHtTrendlineHub();
    }


    /// <summary>
    /// Creates a Hurst hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="HurstHub"/>.</returns>
    public static HurstHub ToHurstHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 100)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHurstHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates an Ichimoku hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <returns>An instance of <see cref="IchimokuHub"/>.</returns>
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

    /// <summary>
    /// Creates an Ichimoku hub from a collection of quotes with specified parameters.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="offsetPeriods">The number of periods for the offset.</param>
    /// <returns>An instance of <see cref="IchimokuHub"/>.</returns>
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


    /// <summary>
    /// Creates a Kama hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="erPeriods">Parameter for the calculation.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="KamaHub"/>.</returns>
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


    /// <summary>
    /// Creates a Macd hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <param name="signalPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="MacdHub"/>.</returns>
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


    /// <summary>
    /// Creates a MAMA hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="fastLimit">Parameter for the calculation.</param>
    /// <param name="slowLimit">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="MamaHub"/>.</returns>
    public static MamaHub ToMamaHub(
        this IReadOnlyList<IQuote> quotes,
        double fastLimit = 0.5,
        double slowLimit = 0.05)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMamaHub(fastLimit, slowLimit);
    }


    /// <summary>
    /// Creates a Marubozu hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="minBodyPercent">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="MarubozuHub"/>.</returns>
    public static MarubozuHub ToMarubozuHub(
        this IReadOnlyList<IQuote> quotes,
        double minBodyPercent = 95)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMarubozuHub(minBodyPercent);
    }


    /// <summary>
    /// Creates an Mfi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of lookback periods. Default is 14.</param>
    /// <returns>An instance of <see cref="MfiHub"/>.</returns>
    public static MfiHub ToMfiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMfiHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates an Obv hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>An instance of <see cref="ObvHub"/>.</returns>
    public static ObvHub ToObvHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToObvHub();
    }


    /// <summary>
    /// Creates a Parabolic SAR hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="accelerationStep">The acceleration step for the SAR calculation. Default is 0.02.</param>
    /// <param name="maxAccelerationFactor">The maximum acceleration factor for the SAR calculation. Default is 0.2.</param>
    /// <returns>An instance of <see cref="ParabolicSarHub"/>.</returns>
    public static ParabolicSarHub ToParabolicSarHub(
        this IReadOnlyList<IQuote> quotes,
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToParabolicSarHub(accelerationStep, maxAccelerationFactor);
    }

    /// <summary>
    /// Creates a Parabolic SAR hub from a collection of quotes with custom initial factor.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="accelerationStep">The acceleration step for the SAR calculation.</param>
    /// <param name="maxAccelerationFactor">The maximum acceleration factor for the SAR calculation.</param>
    /// <param name="initialFactor">The initial acceleration factor for the SAR calculation.</param>
    /// <returns>An instance of <see cref="ParabolicSarHub"/>.</returns>
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


    /// <summary>
    /// Creates a PivotPoints hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="windowSize">The size of the window for pivot calculation. Default is <see cref="PeriodSize.Month"/>.</param>
    /// <param name="pointType">The type of pivot points to calculate. Default is <see cref="PivotPointType.Standard"/>.</param>
    /// <returns>An instance of <see cref="PivotPointsHub"/>.</returns>
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


    /// <summary>
    /// Creates a Pivots hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="leftSpan">The number of periods to the left of the pivot point. Default is 2.</param>
    /// <param name="rightSpan">The number of periods to the right of the pivot point. Default is 2.</param>
    /// <param name="maxTrendPeriods">The maximum number of periods for trend calculation. Default is 20.</param>
    /// <param name="endType">The type of end point for the pivot calculation. Default is <see cref="EndType.HighLow"/>.</param>
    /// <returns>An instance of <see cref="PivotsHub"/>.</returns>
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


    /// <summary>
    /// Creates a PMO hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="timePeriods">The number of periods for the time span.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>An instance of <see cref="PmoHub"/>.</returns>
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


    /// <summary>
    /// Creates a Pvo hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <param name="signalPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="PvoHub"/>.</returns>
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


    /// <summary>
    /// Creates a new QuoteHub from an initiating collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public static QuoteHub ToQuoteHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();  // cannot dogfood ToQuoteHub() here
        quoteHub.Add(quotes);
        return quoteHub;
    }


    /// <summary>
    /// Creates a QuotePart hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="candlePart">The <see cref="CandlePart" /> element.</param>
    /// <returns>An new <see cref="QuotePartHub"/> instance.</returns>
    public static QuotePartHub ToQuotePartHub(
        this IReadOnlyList<IQuote> quotes,
        CandlePart candlePart)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToQuotePartHub(candlePart);
    }


    /// <summary>
    /// Creates a Renko hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="brickSize">Parameter for the calculation.</param>
    /// <param name="endType">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="RenkoHub"/>.</returns>
    public static RenkoHub ToRenkoHub(
        this IReadOnlyList<IQuote> quotes,
        decimal brickSize,
        EndType endType = EndType.Close)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRenkoHub(brickSize, endType);
    }


    /// <summary>
    /// Creates a Roc hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="RocHub"/>.</returns>
    public static RocHub ToRocHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRocHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a RocWb hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window for ROC.</param>
    /// <param name="emaPeriods">Quantity of periods for EMA smoothing.</param>
    /// <param name="stdDevPeriods">Quantity of periods for standard deviation bands.</param>
    /// <returns>An instance of <see cref="RocWbHub"/>.</returns>
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


    /// <summary>
    /// Creates a Rsi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="RsiHub"/>.</returns>
    public static RsiHub ToRsiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRsiHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Slope hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="SlopeHub"/>.</returns>
    public static SlopeHub ToSlopeHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSlopeHub(lookbackPeriods);

        // reminder: can't be self-ref 'quotes.ToHub' syntax
    }


    /// <summary>
    /// Creates an SMA Analysis hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="SmaAnalysisHub"/>.</returns>
    public static SmaAnalysisHub ToSmaAnalysisHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmaAnalysisHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates an SMA hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="SmaHub"/>.</returns>
    public static SmaHub ToSmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmaHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Stochastic Momentum Index (SMI) hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback window.</param>
    /// <param name="firstSmoothPeriods">The number of periods for the first smoothing.</param>
    /// <param name="secondSmoothPeriods">The number of periods for the second smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line smoothing.</param>
    /// <returns>An instance of <see cref="SmiHub"/>.</returns>
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


    /// <summary>
    /// Creates a Smma hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="SmmaHub"/>.</returns>
    public static SmmaHub ToSmmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmmaHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Schaff Trend Cycle (STC) hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="cyclePeriods">Parameter for the calculation.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="StcHub"/> representing the STC indicator.</returns>
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


    /// <summary>
    /// Creates a StdDev hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="StdDevHub"/>.</returns>
    public static StdDevHub ToStdDevHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStdDevHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Stoch hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <param name="signalPeriods">Parameter for the calculation.</param>
    /// <param name="smoothPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="StochHub"/>.</returns>
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


    /// <summary>
    /// Creates a StochRsi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <returns>An instance of <see cref="StochRsiHub"/>.</returns>
    public static StochRsiHub ToStochRsiHub(
        this IReadOnlyList<IQuote> quotes, int rsiPeriods = 14, int stochPeriods = 14, int signalPeriods = 3, int smoothPeriods = 1)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStochRsiHub(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);
    }


    /// <summary>
    /// Creates a SuperTrend hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="SuperTrendHub"/>.</returns>
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


    /// <summary>
    /// Creates a T3 hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <param name="volumeFactor">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="T3Hub"/>.</returns>
    public static T3Hub ToT3Hub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToT3Hub(lookbackPeriods, volumeFactor);
    }


    /// <summary>
    /// Creates a Tema hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="TemaHub"/>.</returns>
    public static TemaHub ToTemaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTemaHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Tr hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>An instance of <see cref="TrHub"/>.</returns>
    public static TrHub ToTrHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTrHub();
    }


    /// <summary>
    /// Creates a Trix hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="TrixHub"/>.</returns>
    public static TrixHub ToTrixHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTrixHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a TSI hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation.</param>
    /// <returns>An instance of <see cref="TsiHub"/>.</returns>
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


    /// <summary>
    /// Creates an Ulcer Index hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="UlcerIndexHub"/>.</returns>
    public static UlcerIndexHub ToUlcerIndexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToUlcerIndexHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Ultimate hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="shortPeriods">Parameter for the calculation.</param>
    /// <param name="middlePeriods">Parameter for the calculation.</param>
    /// <param name="longPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="UltimateHub"/>.</returns>
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


    /// <summary>
    /// Creates a VolatilityStop hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <returns>An instance of <see cref="VolatilityStopHub"/>.</returns>
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


    /// <summary>
    /// Creates a Vortex hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="VortexHub"/>.</returns>
    public static VortexHub ToVortexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVortexHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a VWAP hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="startDate">The start date for VWAP calculation. If null, auto-anchors to first quote.</param>
    /// <returns>An instance of <see cref="VwapHub"/>.</returns>
    public static VwapHub ToVwapHub(
        this IReadOnlyList<IQuote> quotes,
        DateTime? startDate = null)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVwapHub(startDate);
    }


    /// <summary>
    /// Creates a Vwma hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="VwmaHub"/>.</returns>
    public static VwmaHub ToVwmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToVwmaHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a WilliamsR hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="WilliamsRHub"/>.</returns>
    public static WilliamsRHub ToWilliamsRHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWilliamsRHub(lookbackPeriods);
    }


    /// <summary>
    /// Creates a Wma hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="WmaHub"/>.</returns>
    public static WmaHub ToWmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWmaHub(lookbackPeriods);
    }
}
