namespace Skender.Stock.Indicators;

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
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
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
public class MacdHub<TIn>
    : ChainProvider<TIn, MacdResult>, IMacd
    where TIn : IReusable
{
    private readonly string hubName;
    private double? _lastFastEma;
    private double? _lastSlowEma;
    private double? _lastSignalEma;

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
        double? fastEma = null;
        if (i >= FastPeriods - 1)
        {
            if (_lastFastEma is null)
            {
                // Initialize as SMA
                fastEma = Sma.Increment(ProviderCache, FastPeriods, i);
            }
            else
            {
                // Calculate EMA normally
                fastEma = Ema.Increment(FastK, _lastFastEma.Value, item.Value);
            }
            _lastFastEma = fastEma;
        }

        // Calculate Slow EMA
        double? slowEma = null;
        if (i >= SlowPeriods - 1)
        {
            if (_lastSlowEma is null)
            {
                // Initialize as SMA
                slowEma = Sma.Increment(ProviderCache, SlowPeriods, i);
            }
            else
            {
                // Calculate EMA normally
                slowEma = Ema.Increment(SlowK, _lastSlowEma.Value, item.Value);
            }
            _lastSlowEma = slowEma;
        }

        // Calculate MACD
        double? macd = null;
        if (fastEma.HasValue && slowEma.HasValue)
        {
            macd = fastEma.Value - slowEma.Value;
        }

        // Calculate Signal
        double? signal = null;
        if (macd.HasValue && i >= SlowPeriods + SignalPeriods - 2)
        {
            if (_lastSignalEma is null)
            {
                // Initialize signal as SMA of MACD values
                double sum = macd.Value;
                for (int j = Math.Max(0, i - SignalPeriods + 1); j < i; j++)
                {
                    if (Cache[j].Macd.HasValue)
                    {
                        sum += Cache[j].Macd!.Value;
                    }
                }
                signal = sum / SignalPeriods;
            }
            else
            {
                // Calculate signal EMA normally
                signal = Ema.Increment(SignalK, _lastSignalEma.Value, macd.Value);
            }
            _lastSignalEma = signal;
        }

        // Calculate Histogram
        double? histogram = null;
        if (macd.HasValue && signal.HasValue)
        {
            histogram = macd.Value - signal.Value;
        }

        // Candidate result
        MacdResult r = new(
            Timestamp: item.Timestamp,
            Macd: macd,
            Signal: signal,
            Histogram: histogram,
            FastEma: fastEma,
            SlowEma: slowEma);

        return (r, i);
    }
}
