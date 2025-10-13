namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Chaikin Oscillator.
/// </summary>
public class ChaikinOscHub<TIn>
    : ChainProvider<TIn, ChaikinOscResult>, IChaikinOsc
    where TIn : IQuote
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinOscHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the periods are invalid.</exception>
    internal ChaikinOscHub(
        IChainProvider<TIn> provider,
        int fastPeriods,
        int slowPeriods) : base(provider)
    {
        ChaikinOsc.Validate(fastPeriods, slowPeriods);
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;

        FastK = 2d / (fastPeriods + 1);
        SlowK = 2d / (slowPeriods + 1);

        hubName = $"CHAIKIN_OSC({fastPeriods},{slowPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int FastPeriods { get; init; }

    /// <inheritdoc/>
    public int SlowPeriods { get; init; }

    /// <summary>
    /// Gets the smoothing factor for the fast EMA.
    /// </summary>
    public double FastK { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the slow EMA.
    /// </summary>
    public double SlowK { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (ChaikinOscResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        TIn quote = ProviderCache[i];

        // Calculate Money Flow Multiplier and Money Flow Volume (ADL components)
        double high = (double)quote.High;
        double low = (double)quote.Low;
        double close = (double)quote.Close;
        double volume = (double)quote.Volume;

        double? mfm = null;
        double? mfv = null;
        double range = high - low;

        if (range > 0)
        {
            mfm = (close - low - (high - close)) / range;
            mfv = mfm * volume;
        }

        // Calculate ADL (cumulative)
        double adl = i > 0 && Cache[i - 1].Adl.HasValue
            ? Cache[i - 1].Adl!.Value + (mfv ?? 0)
            : mfv ?? 0;

        // Calculate Fast EMA of ADL
        double fastEma = i >= FastPeriods - 1
            ? i > 0 && Cache[i - 1].FastEma is not null
                // Calculate EMA normally
                ? Ema.Increment(FastK, Cache[i - 1].FastEma!.Value, adl)
                // Initialize as SMA of ADL values
                : CalculateInitialSma(i, FastPeriods, adl)
            // warmup periods are never calculable
            : double.NaN;

        // Calculate Slow EMA of ADL
        double slowEma = i >= SlowPeriods - 1
            ? i > 0 && Cache[i - 1].SlowEma is not null
                // Calculate EMA normally
                ? Ema.Increment(SlowK, Cache[i - 1].SlowEma!.Value, adl)
                // Initialize as SMA of ADL values
                : CalculateInitialSma(i, SlowPeriods, adl)
            // warmup periods are never calculable
            : double.NaN;

        // Calculate Oscillator
        double? oscillator = (fastEma - slowEma).NaN2Null();

        // Candidate result
        ChaikinOscResult r = new(
            Timestamp: item.Timestamp,
            MoneyFlowMultiplier: mfm,
            MoneyFlowVolume: mfv,
            Adl: adl,
            Oscillator: oscillator,
            FastEma: fastEma.NaN2Null(),
            SlowEma: slowEma.NaN2Null());

        return (r, i);
    }

    private double CalculateInitialSma(int currentIndex, int periods, double currentAdl)
    {
        // Check if we have enough data
        if (currentIndex < periods - 1)
        {
            return double.NaN;
        }

        // Sum ADL values from Cache (which has entries 0 through currentIndex-1)
        // plus the current ADL value
        double sum = currentAdl;
        for (int j = currentIndex - periods + 1; j < currentIndex; j++)
        {
            if (j >= 0 && j < Cache.Count)
            {
                sum += Cache[j].Adl ?? 0;
            }
        }

        return sum / periods;
    }
}


public static partial class ChaikinOsc
{
    /// <summary>
    /// Creates a Chaikin Oscillator hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the quote data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 3.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 10.</param>
    /// <returns>A Chaikin Oscillator hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the periods are invalid.</exception>
    public static ChaikinOscHub<T> ToChaikinOscHub<T>(
        this IChainProvider<T> chainProvider,
        int fastPeriods = 3,
        int slowPeriods = 10)
        where T : IQuote
        => new(chainProvider, fastPeriods, slowPeriods);

    /// <summary>
    /// Creates a ChaikinOsc hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="ChaikinOscHub{TQuote}"/>.</returns>
    public static ChaikinOscHub<TQuote> ToChaikinOscHub<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int fastPeriods = 3,
        int slowPeriods = 10)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToChaikinOscHub(fastPeriods, slowPeriods);
    }

}
