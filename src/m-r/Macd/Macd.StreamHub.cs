namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for MACD (Moving Average Convergence Divergence) calculations.
/// </summary>
public interface IMacd
{
    /// <summary>
    /// Gets the number of periods for the fast EMA.
    /// </summary>
    int FastPeriods { get; }

    /// <summary>
    /// Gets the number of periods for the slow EMA.
    /// </summary>
    int SlowPeriods { get; }

    /// <summary>
    /// Gets the number of periods for the signal line.
    /// </summary>
    int SignalPeriods { get; }
}

/// <summary>
/// Provides methods for calculating the MACD (Moving Average Convergence Divergence) indicator.
/// </summary>
public static partial class Macd
{
    /// <summary>
    /// Creates a MACD hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 12.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 26.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 9.</param>
    /// <returns>A MACD hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static MacdHub<T> ToMacd<T>(
        this IChainProvider<T> chainProvider,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where T : IReusable
        => new(chainProvider, fastPeriods, slowPeriods, signalPeriods);
}

/// <summary>
/// Represents a hub for MACD (Moving Average Convergence Divergence) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class MacdHub<TIn> : ChainProvider<TIn, MacdResult>, IMacd
    where TIn : IReusable
{
    private readonly string hubName;
    
    private double lastEmaFast = double.NaN;
    private double lastEmaSlow = double.NaN;
    private double lastEmaMacd = double.NaN;
    
    private readonly double kFast;
    private readonly double kSlow;
    private readonly double kMacd;

    /// <summary>
    /// Initializes a new instance of the <see cref="MacdHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    internal MacdHub(
        IChainProvider<TIn> provider,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods) : base(provider)
    {
        // validate parameters
        Macd.Validate(fastPeriods, slowPeriods, signalPeriods);
        
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        SignalPeriods = signalPeriods;

        // calculate smoothing factors
        kFast = 2d / (fastPeriods + 1);
        kSlow = 2d / (slowPeriods + 1);
        kMacd = 2d / (signalPeriods + 1);

        hubName = $"MACD({fastPeriods},{slowPeriods},{signalPeriods})";
        
        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods for the fast EMA.
    /// </summary>
    public int FastPeriods { get; }

    /// <summary>
    /// Gets the number of periods for the slow EMA.
    /// </summary>
    public int SlowPeriods { get; }

    /// <summary>
    /// Gets the number of periods for the signal line.
    /// </summary>
    public int SignalPeriods { get; }

    /// <inheritdoc/>
    protected override (MacdResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Fast EMA
        double emaFast
            = i >= FastPeriods - 1 && i > 0 && Cache[i - 1].FastEma is null
            ? Sma.Increment(ProviderCache, FastPeriods, i)
            : Ema.Increment(kFast, lastEmaFast, item.Value);

        lastEmaFast = emaFast;

        // Slow EMA
        double emaSlow
            = i >= SlowPeriods - 1 && i > 0 && Cache[i - 1].SlowEma is null
            ? Sma.Increment(ProviderCache, SlowPeriods, i)
            : Ema.Increment(kSlow, lastEmaSlow, item.Value);

        lastEmaSlow = emaSlow;

        // MACD line
        double macd = emaFast - emaSlow;

        // Signal line
        double signal;

        if (i >= SignalPeriods + SlowPeriods - 2 && i > 0 && Cache[i - 1].Signal is null)
        {
            // Initialize signal with SMA of MACD values
            double sum = macd;
            for (int p = i - SignalPeriods + 1; p < i; p++)
            {
                sum += Cache[p].Value; // Cache[p].Value is the MACD value (IReusable.Value)
            }
            signal = sum / SignalPeriods;
        }
        else
        {
            signal = Ema.Increment(kMacd, lastEmaMacd, macd);
        }

        lastEmaMacd = signal;

        // Histogram
        double histogram = macd - signal;

        MacdResult result = new(
            item.Timestamp,
            macd.NaN2Null(),
            signal.NaN2Null(),
            histogram.NaN2Null(),
            i >= FastPeriods - 1 ? emaFast.NaN2Null() : null,
            i >= SlowPeriods - 1 ? emaSlow.NaN2Null() : null);

        return (result, i);
    }

    /// <inheritdoc/>
    public override string ToString() => hubName;
}