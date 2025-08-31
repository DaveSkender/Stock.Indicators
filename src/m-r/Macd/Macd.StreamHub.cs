namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for creating MACD hubs.
/// </summary>
public static partial class Macd
{
    /// <summary>
    /// Converts the chain provider to a MACD hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 12.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 26.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 9.</param>
    /// <returns>A MACD hub.</returns>
    public static MacdHub<TIn> ToMacd<TIn>(
        this IChainProvider<TIn> chainProvider,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
        where TIn : IReusable
        => new(chainProvider, fastPeriods, slowPeriods, signalPeriods);
}

/// <summary>
/// Represents a MACD (Moving Average Convergence Divergence) stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class MacdHub<TIn> : IndicatorStreamHubBase<TIn, MacdResult>, IMultiPeriodIndicator
    where TIn : IReusable
{
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
    internal MacdHub(
        IChainProvider<TIn> provider,
        int fastPeriods,
        int slowPeriods,
        int signalPeriods) : base(provider, "MACD", fastPeriods, slowPeriods, signalPeriods)
    {
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        SignalPeriods = signalPeriods;
        LookbackPeriods = slowPeriods; // Use slow periods as the primary lookback

        kFast = 2d / (fastPeriods + 1);
        kSlow = 2d / (slowPeriods + 1);
        kMacd = 2d / (signalPeriods + 1);

        // Initialize with validation
        Initialize();
    }

    /// <inheritdoc/>
    public override int LookbackPeriods { get; }

    /// <inheritdoc/>
    public int FastPeriods { get; }

    /// <inheritdoc/>
    public int SlowPeriods { get; }

    /// <inheritdoc/>
    public int SignalPeriods { get; }

    /// <inheritdoc/>
    protected override void ValidateParameters()
    {
        IndicatorUtilities.ValidateMultiPeriods(FastPeriods, SlowPeriods, SignalPeriods, "MACD");
    }

    /// <inheritdoc/>
    protected override (MacdResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Fast EMA
        double emaFast = i >= FastPeriods - 1
            ? Cache[i - 1].FastEma is null
                ? Sma.Increment(ProviderCache, FastPeriods, i)
                : Ema.Increment(kFast, Cache[i - 1].FastEma!.Value, item.Value)
            : double.NaN;

        // Slow EMA
        double emaSlow = i >= SlowPeriods - 1
            ? Cache[i - 1].SlowEma is null
                ? Sma.Increment(ProviderCache, SlowPeriods, i)
                : Ema.Increment(kSlow, Cache[i - 1].SlowEma!.Value, item.Value)
            : double.NaN;

        // MACD
        double macd = emaFast - emaSlow;

        // Signal
        double signal = double.NaN;
        if (i >= SignalPeriods + SlowPeriods - 2)
        {
            if (Cache[i - 1].Signal is null)
            {
                // Initialize signal with SMA of MACD values
                double sum = macd;
                for (int p = i - SignalPeriods + 1; p < i; p++)
                {
                    sum += Cache[p].Value;
                }
                signal = sum / SignalPeriods;
            }
            else
            {
                signal = Ema.Increment(kMacd, Cache[i - 1].Signal!.Value, macd);
            }
        }

        // Histogram
        double histogram = macd - signal;

        // Candidate result
        MacdResult r = new(
            Timestamp: item.Timestamp,
            Macd: macd.NaN2Null(),
            Signal: signal.NaN2Null(),
            Histogram: histogram.NaN2Null(),
            FastEma: emaFast.NaN2Null(),
            SlowEma: emaSlow.NaN2Null());

        return (r, i);
    }
}