namespace Skender.Stock.Indicators;

// CHAIKIN OSCILLATOR (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Chaikin Oscillator.
/// </summary>
public class ChaikinOscHub
    : ChainProvider<IQuote, ChaikinOscResult>, IChaikinOsc
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinOscHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the periods are invalid.</exception>
    internal ChaikinOscHub(
        IQuoteProvider<IQuote> provider,
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
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        IQuote quote = ProviderCache[i];

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
        double fastEma;
        if (i >= FastPeriods - 1)
        {
            if (i > 0 && Cache[i - 1].FastEma is not null)
            {
                fastEma = Ema.Increment(FastK, Cache[i - 1].FastEma!.Value, adl);
            }
            else
            {
                fastEma = CalculateInitialSma(i, FastPeriods, adl);
            }
        }
        else
        {
            fastEma = double.NaN;
        }

        // Calculate Slow EMA of ADL
        double slowEma;
        if (i >= SlowPeriods - 1)
        {
            if (i > 0 && Cache[i - 1].SlowEma is not null)
            {
                slowEma = Ema.Increment(SlowK, Cache[i - 1].SlowEma!.Value, adl);
            }
            else
            {
                slowEma = CalculateInitialSma(i, SlowPeriods, adl);
            }
        }
        else
        {
            slowEma = double.NaN;
        }

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
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 3.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 10.</param>
    /// <returns>A Chaikin Oscillator hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the periods are invalid.</exception>
    public static ChaikinOscHub ToChaikinOscHub(
        this IQuoteProvider<IQuote> chainProvider,
        int fastPeriods = 3,
        int slowPeriods = 10)
        => new(chainProvider, fastPeriods, slowPeriods);

    /// <summary>
    /// Creates a ChaikinOsc hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
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

}
