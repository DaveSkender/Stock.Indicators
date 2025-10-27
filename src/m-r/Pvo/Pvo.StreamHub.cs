namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the PVO (Percentage Volume Oscillator) indicator.
/// </summary>
public class PvoHub
    : ChainProvider<IReusable, PvoResult>, IPvo
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="PvoHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    internal PvoHub(
        IChainProvider<IReusable> provider,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods) : base(provider)
    {
        Pvo.Validate(fastPeriods, slowPeriods, signalPeriods);
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        SignalPeriods = signalPeriods;

        FastK = 2d / (fastPeriods + 1);
        SlowK = 2d / (slowPeriods + 1);
        SignalK = 2d / (signalPeriods + 1);

        hubName = $"PVO({fastPeriods},{slowPeriods},{signalPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int FastPeriods { get; init; }

    /// <inheritdoc/>
    public int SlowPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <summary>
    /// Gets the smoothing factor for the fast EMA.
    /// </summary>
    public double FastK { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the slow EMA.
    /// </summary>
    public double SlowK { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the signal line.
    /// </summary>
    public double SignalK { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (PvoResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate Fast EMA
        double fastEma = i >= FastPeriods - 1
            ? i > 0 && Cache[i - 1].FastEma is not null
                // Calculate EMA normally
                ? Ema.Increment(FastK, Cache[i - 1].FastEma!.Value, item.Value)
                // Initialize as SMA
                : Sma.Increment(ProviderCache, FastPeriods, i)
            // warmup periods are never calculable
            : double.NaN;

        // Calculate Slow EMA
        double slowEma = i >= SlowPeriods - 1
            ? i > 0 && Cache[i - 1].SlowEma is not null
                // Calculate EMA normally
                ? Ema.Increment(SlowK, Cache[i - 1].SlowEma!.Value, item.Value)
                // Initialize as SMA
                : Sma.Increment(ProviderCache, SlowPeriods, i)
            // warmup periods are never calculable
            : double.NaN;

        // Calculate PVO
        double pvo = slowEma != 0 ? 100 * ((fastEma - slowEma) / slowEma) : double.NaN;

        // Calculate Signal
        double signal;
        if (i >= SignalPeriods + SlowPeriods - 2 && (i == 0 || Cache[i - 1].Signal is null))
        {
            // Initialize signal as SMA of PVO values
            double sum = pvo;
            for (int j = i - SignalPeriods + 1; j < i; j++)
            {
                sum += Cache[j].Value;
            }

            signal = sum / SignalPeriods;
        }
        else
        {
            // Calculate signal EMA normally
            signal = Ema.Increment(SignalK, i > 0 ? Cache[i - 1].Signal ?? double.NaN : double.NaN, pvo);
        }

        // Candidate result
        PvoResult r = new(
            Timestamp: item.Timestamp,
            Pvo: pvo.NaN2Null(),
            Signal: signal.NaN2Null(),
            Histogram: (pvo - signal).NaN2Null(),
            FastEma: fastEma.NaN2Null(),
            SlowEma: slowEma.NaN2Null());

        return (r, i);
    }
}


public static partial class Pvo
{
    /// <summary>
    /// Creates a PVO streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 12.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 26.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 9.</param>
    /// <returns>A PVO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static PvoHub ToPvoHub(
        this IChainProvider<IReusable> chainProvider,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        => new(chainProvider, fastPeriods, slowPeriods, signalPeriods);

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
        return quoteHub
            .ToQuotePartHub(CandlePart.Volume)
            .ToPvoHub(fastPeriods, slowPeriods, signalPeriods);
    }

}
