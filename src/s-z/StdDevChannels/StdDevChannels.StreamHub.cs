namespace Skender.Stock.Indicators;

// STANDARD DEVIATION CHANNELS (STREAM HUB)

/// <summary>
/// Interface for Standard Deviation Channels Hub.
/// </summary>
public interface IStdDevChannels
{
    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    int LookbackPeriods { get; }

    /// <summary>
    /// Gets the number of standard deviations.
    /// </summary>
    double StandardDeviations { get; }
}

/// <summary>
/// Provides methods for creating Standard Deviation Channels hubs.
/// </summary>
/// <remarks>
/// <para>
/// Standard Deviation Channels uses a reverse window algorithm that recalculates
/// ALL values when new data arrives. This is different from typical streaming
/// indicators where each value is calculated once. As the cache grows, window
/// boundaries shift, causing values to "repaint" (change retrospectively).
/// </para>
/// <para>
/// Performance: Incremental Slope calculation reduces complexity from O(n³) to O(n²).
/// Forward streaming (Add) is reasonably fast. Provider history mutations (Insert/Remove)
/// trigger full rebuilds which are expensive but unavoidable for correctness.
/// </para>
/// </remarks>
public class StdDevChannelsHub
    : StreamHub<IReusable, StdDevChannelsResult>, IStdDevChannels
{
    #region constructors

    private readonly string hubName;
    private int _lastSyncCount = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="StdDevChannelsHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    internal StdDevChannelsHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double standardDeviations) : base(provider)
    {
        StdDevChannels.Validate(lookbackPeriods, standardDeviations);
        LookbackPeriods = lookbackPeriods;
        StandardDeviations = standardDeviations;
        hubName = $"STDEV-CHANNELS({lookbackPeriods},{standardDeviations})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of standard deviations.
    /// </summary>
    public double StandardDeviations { get; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <summary>
    /// Resets sync state after provider history mutations (Insert/Remove).
    /// </summary>
    /// <remarks>
    /// Standard Deviation Channels recalculates the entire series on the next ToIndicator call
    /// after provider history changes. This override ensures the sync counter is reset so the
    /// recalculation is triggered.
    /// </remarks>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset sync counter to force full recalculation
        _lastSyncCount = -1;
    }

    /// <summary>
    /// Converts provider item into Standard Deviation Channels result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Standard Deviation Channels is a repaint-by-design indicator where window values
    /// change as new data arrives because window boundaries shift with dataset growth.
    /// </para>
    /// <para>
    /// Performance: Only calculates Slope for new windows and applies channels to the
    /// lookback period, not the entire dataset. Provider history mutations (Insert/Remove)
    /// trigger full recalculation via RollbackState.
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override (StdDevChannelsResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        int currentCount = ProviderCache.Count;

        // Handle rollback (Insert/Remove) - full recalculation needed
        if (_lastSyncCount == -1)
        {
            IReadOnlyList<StdDevChannelsResult> seriesResults =
                ProviderCache.ToStdDevChannels(LookbackPeriods, StandardDeviations);

            Cache.Clear();
            Cache.AddRange(seriesResults);
            _lastSyncCount = currentCount;

            return (Cache[i], i);
        }

        // Add new result placeholder if cache needs to grow
        if (Cache.Count < currentCount)
        {
            Cache.Add(new StdDevChannelsResult(item.Timestamp));
        }

        // Check if we have enough data for a new window at current index
        if (i >= LookbackPeriods - 1 && (i + 1) % LookbackPeriods == 0)
        {
            // Calculate Slope for this window endpoint
            SlopeResult slope = CalculateWindowSlope(i);
            double? width = StandardDeviations * slope.StdDev;

            // Apply channels to the entire window
            int windowStart = i - LookbackPeriods + 1;
            for (int p = windowStart; p <= i; p++)
            {
                double? c = (slope.Slope * (p + 1)) + slope.Intercept;

                Cache[p] = Cache[p] with {
                    Centerline = c,
                    UpperChannel = c + width,
                    LowerChannel = c - width,
                    BreakPoint = p == windowStart
                };
            }
        }

        _lastSyncCount = currentCount;
        return (Cache[i], i);
    }

    /// <summary>
    /// Calculates Slope result for a window ending at the specified index.
    /// </summary>
    private SlopeResult CalculateWindowSlope(int endIndex)
    {
        IReusable item = ProviderCache[endIndex];

        // Get averages for window
        double sumX = 0;
        double sumY = 0;
        int windowStart = endIndex - LookbackPeriods + 1;

        for (int p = windowStart; p <= endIndex; p++)
        {
            IReusable ps = ProviderCache[p];
            sumX += p + 1d;
            sumY += ps.Value;
        }

        double avgX = sumX / LookbackPeriods;
        double avgY = sumY / LookbackPeriods;

        // Least squares regression
        double sumSqX = 0;
        double sumSqY = 0;
        double sumSqXy = 0;

        for (int p = windowStart; p <= endIndex; p++)
        {
            IReusable ps = ProviderCache[p];
            double devX = p + 1d - avgX;
            double devY = ps.Value - avgY;

            sumSqX += devX * devX;
            sumSqY += devY * devY;
            sumSqXy += devX * devY;
        }

        double? slope = (sumSqXy / sumSqX).NaN2Null();
        double? intercept = (avgY - (slope * avgX)).NaN2Null();
        double stdDevY = Math.Sqrt(sumSqY / LookbackPeriods);

        return new SlopeResult(item.Timestamp) {
            Slope = slope,
            Intercept = intercept,
            StdDev = stdDevY.NaN2Null()
        };
    }

    #endregion methods
}


public static partial class StdDevChannels
{
    /// <summary>
    /// Converts the chain provider to a Standard Deviation Channels hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    /// <returns>A Standard Deviation Channels hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static StdDevChannelsHub ToStdDevChannelsHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
             => new(chainProvider, lookbackPeriods, standardDeviations);

    /// <summary>
    /// Creates a Standard Deviation Channels hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">Number of standard deviations for the channels.</param>
    /// <returns>An instance of <see cref="StdDevChannelsHub"/>.</returns>
    public static StdDevChannelsHub ToStdDevChannelsHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
             => new(quoteProvider, lookbackPeriods, standardDeviations);

    /// <summary>
    /// Creates a Standard Deviation Channels hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">Number of standard deviations for the channels.</param>
    /// <returns>An instance of <see cref="StdDevChannelsHub"/>.</returns>
    public static StdDevChannelsHub ToStdDevChannelsHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStdDevChannelsHub(lookbackPeriods, standardDeviations);
    }
}
