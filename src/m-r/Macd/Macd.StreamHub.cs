namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the MACD (Moving Average Convergence Divergence) indicator.
/// </summary>
public class MacdHub<TIn>
    : ChainProvider<TIn, MacdResult>, IMacd
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="MacdHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    internal MacdHub(
        IChainProvider<TIn> provider,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods) : base(provider)
    {
        Macd.Validate(fastPeriods, slowPeriods, signalPeriods);
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        SignalPeriods = signalPeriods;

        FastK = 2d / (fastPeriods + 1);
        SlowK = 2d / (slowPeriods + 1);
        SignalK = 2d / (signalPeriods + 1);

        hubName = $"MACD({fastPeriods},{slowPeriods},{signalPeriods})";

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
    protected override (MacdResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
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

        // Calculate MACD
        double macd = fastEma - slowEma;

        // Calculate Signal
        double signal;
        if (i >= SignalPeriods + SlowPeriods - 2 && (i == 0 || Cache[i - 1].Signal is null))
        {
            // Initialize signal as SMA of MACD values
            double sum = macd;
            for (int j = i - SignalPeriods + 1; j < i; j++)
            {
                sum += Cache[j].Value;
            }

            signal = sum / SignalPeriods;
        }
        else
        {
            // Calculate signal EMA normally
            signal = Ema.Increment(SignalK, i > 0 ? Cache[i - 1].Signal ?? double.NaN : double.NaN, macd);
        }

        // Candidate result
        MacdResult r = new(
            Timestamp: item.Timestamp,
            Macd: macd.NaN2Null(),
            Signal: signal.NaN2Null(),
            Histogram: (macd - signal).NaN2Null(),
            FastEma: fastEma.NaN2Null(),
            SlowEma: slowEma.NaN2Null());

        return (r, i);
    }
}


public static partial class Macd
{
    /// <summary>
    /// Creates a MACD streaming hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 12.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 26.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 9.</param>
    /// <returns>A MACD hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static MacdHub<T> ToMacdHub<T>(
        this IChainProvider<T> chainProvider,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where T : IReusable
        => new(chainProvider, fastPeriods, slowPeriods, signalPeriods);

    /// <summary>
    /// Creates a Macd hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <param name="signalPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="MacdHub{TQuote}"/>.</returns>
    public static MacdHub<TQuote> ToMacdHub<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMacdHub(fastPeriods, slowPeriods, signalPeriods);
    }

}
