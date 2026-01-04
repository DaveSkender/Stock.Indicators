namespace Skender.Stock.Indicators;

/// <summary>
/// TDIGM (Traders Dynamic Index [Goldminds]) streaming hub implementation.
/// Derives from ChainHub to enable streaming computation and chaining following Skender v3 pattern.
/// 
/// Computation pipeline using chained hubs:
/// 1. RsiHub(rsiPeriod) on input quotes → RSI values
/// 2. SmaHub(bandLength) on RSI values → middle band
/// 3. StdDevHub(bandLength) on RSI values → used to calculate upper/lower bands with 1.6185 multiplier
/// 4. SmaHub(fastLength) on RSI values → fast MA
/// 5. SmaHub(slowLength) on RSI values → slow MA
/// </summary>
public sealed class TdiGmHub
    : ChainHub<IReusable, TdiGmResult>, ITdiGm
{
    public int RsiPeriod { get; init; }
    public int BandLength { get; init; }
    public int FastLength { get; init; }
    public int SlowLength { get; init; }

    // Chained hubs for efficient computation
    private readonly RsiHub _rsiHub;
    private readonly SmaHub _middleBandHub;  // SMA of RSI values
    private readonly StdDevHub _stdDevHub;   // StdDev of RSI values
    private readonly SmaHub _fastMaHub;      // Fast SMA of RSI values
    private readonly SmaHub _slowMaHub;      // Slow SMA of RSI values

    /// <summary>
    /// Creates a new TdiGmHub instance. Internal constructor - use ToTdiGmHub() extension method.
    /// </summary>
    /// <param name="provider">The chain provider to subscribe to</param>
    /// <param name="rsiPeriod">RSI period for the base oscillator (default: 21)</param>
    /// <param name="bandLength">Band length for middle band and standard deviation (default: 34)</param>
    /// <param name="fastLength">Fast moving average period on RSI (default: 2)</param>
    /// <param name="slowLength">Slow moving average period on RSI (default: 7)</param>
    internal TdiGmHub(
        IChainProvider<IReusable> provider,
        int rsiPeriod,
        int bandLength,
        int fastLength,
        int slowLength)
        : base(provider)
    {
        TdiGm.Validate(rsiPeriod, bandLength, fastLength, slowLength);
        RsiPeriod = rsiPeriod;
        BandLength = bandLength;
        FastLength = fastLength;
        SlowLength = slowLength;

        // Create the hub chain:
        // provider (Candle) → RsiHub → [SmaHub(band), StdDevHub(band), SmaHub(fast), SmaHub(slow)]
        _rsiHub = provider.ToRsiHub(rsiPeriod);
        _middleBandHub = _rsiHub.ToSmaHub(bandLength);
        _stdDevHub = _rsiHub.ToStdDevHub(bandLength);
        _fastMaHub = _rsiHub.ToSmaHub(fastLength);
        _slowMaHub = _rsiHub.ToSmaHub(slowLength);

        Reinitialize();
    }

    /// <summary>
    /// Converts an input item to a TdiGmResult indicator value.
    /// Called by StreamHub base class for each item in ProviderCache.
    /// Uses chained hubs - gets current values from each hub's cache.
    /// </summary>
    /// <param name="item">Input item (Candle)</param>
    /// <param name="indexHint">Index hint for optimization</param>
    /// <returns>Tuple of (indicator result, index)</returns>
    protected override (TdiGmResult, int) ToIndicator(IReusable item, int? indexHint)
    {
        int index = indexHint ?? ProviderCache.Count - 1;
        DateTime timestamp = item.Timestamp;

        // Get the latest results from each chained hub
        // Each hub has already computed its value for this index
        var middleBandResult = _middleBandHub.Results[^1];
        var stdDevResult = _stdDevHub.Results[^1];
        var fastMaResult = _fastMaHub.Results[^1];
        var slowMaResult = _slowMaHub.Results[^1];

        // Calculate TDIGM bands using the hub results
        double? upper = null;
        double? lower = null;
        double? middle = null;

        if (middleBandResult?.Sma != null && stdDevResult?.StdDev != null)
        {
            var ma = middleBandResult.Sma.Value;
            var stdDev = stdDevResult.StdDev.Value;
            var offset = 1.6185 * stdDev;

            upper = ma + offset;
            lower = ma - offset;
            middle = ma;
        }

        // Create the result
        var result = new TdiGmResult() {
            Timestamp = timestamp,
            Upper = upper,
            Lower = lower,
            Middle = middle,
            Fast = fastMaResult?.Sma != null ? fastMaResult.Sma.Value : null,
            Slow = slowMaResult?.Sma != null ? slowMaResult.Sma.Value : null
        };

        return (result, index);
    }

    /// <summary>
    /// Binary settings for hub behavior configuration.
    /// </summary>
    public override BinarySettings Properties
    {
        get => base.Properties;
        init => base.Properties = value;
    }

    /// <summary>
    /// Rolls back the hub state to a specific timestamp by rebuilding from that point.
    /// Used to restore indicator state when replaying historical data.
    /// The base implementation and chained hubs handle their own rollback.
    /// </summary>
    /// <param name="timestamp">The timestamp to roll back to</param>
    protected override void RollbackState(DateTime timestamp)
    {
        // Base implementation handles rolling back through ProviderCache
        // Chained hubs will rollback automatically through their own providers
        base.RollbackState(timestamp);
    }

    /// <summary>
    /// Returns a string representation of this hub.
    /// </summary>
    public override string ToString()
    {
        return $"TdiGmHub(RSI={RsiPeriod}, Band={BandLength}, Fast={FastLength}, Slow={SlowLength})";
    }
}

/// <summary>
/// Extension methods for creating TdiGmHub instances.
/// Following Skender v3 pattern where factory method is in the same file as the hub.
/// </summary>
public static class TdiGmHubExtensions
{
    /// <summary>
    /// Creates a TdiGmHub from any IChainProvider source.
    /// Factory method following Skender v3 pattern (e.g., ToRsi, ToSma, etc.)
    /// </summary>
    /// <param name="source">The source chain provider (e.g., QuoteHub)</param>
    /// <param name="rsiPeriod">RSI period for the base oscillator (default: 21)</param>
    /// <param name="bandLength">Band length for middle band and standard deviation (default: 34)</param>
    /// <param name="fastLength">Fast moving average period on RSI (default: 2)</param>
    /// <param name="slowLength">Slow moving average period on RSI (default: 7)</param>
    /// <returns>A new TdiGmHub instance chained to the source</returns>
    public static TdiGmHub ToTdiGmHub(
        this IChainProvider<IReusable> source,
        int rsiPeriod = 21,
        int bandLength = 34,
        int fastLength = 2,
        int slowLength = 7)
    {
        // Create the hub by passing the provider and parameters
        return new TdiGmHub(source, rsiPeriod, bandLength, fastLength, slowLength);
    }
}
