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
    private readonly List<SlopeResult> _slopeCache = [];

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
        // Reset sync counter and clear slope cache to force full recalculation
        _lastSyncCount = -1;
        _slopeCache.Clear();
    }

    /// <summary>
    /// Calculates Slope result for a single period incrementally.
    /// </summary>
    private static SlopeResult CalculateSlope(IReadOnlyList<IReusable> source, int index, int lookbackPeriods)
    {
        IReusable s = source[index];

        // Skip initialization period
        if (index < lookbackPeriods - 1)
        {
            return new SlopeResult(s.Timestamp);
        }

        // Get averages for period
        double sumX = 0;
        double sumY = 0;

        for (int p = index - lookbackPeriods + 1; p <= index; p++)
        {
            IReusable ps = source[p];
            sumX += p + 1d;
            sumY += ps.Value;
        }

        double avgX = sumX / lookbackPeriods;
        double avgY = sumY / lookbackPeriods;

        // Least squares method
        double sumSqX = 0;
        double sumSqY = 0;
        double sumSqXy = 0;

        for (int p = index - lookbackPeriods + 1; p <= index; p++)
        {
            IReusable ps = source[p];
            double devX = p + 1d - avgX;
            double devY = ps.Value - avgY;

            sumSqX += devX * devX;
            sumSqY += devY * devY;
            sumSqXy += devX * devY;
        }

        double? slope = (sumSqXy / sumSqX).NaN2Null();
        double? intercept = (avgY - (slope * avgX)).NaN2Null();

        // Calculate Standard Deviation
        double stdDevY = Math.Sqrt(sumSqY / lookbackPeriods);

        return new SlopeResult(s.Timestamp)
        {
            Slope = slope,
            Intercept = intercept,
            StdDev = stdDevY.NaN2Null()
        };
    }

    /// <summary>
    /// Converts provider item into Standard Deviation Channels result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Standard Deviation Channels is a repaint-by-design indicator where ALL values
    /// may change as new data arrives because window boundaries shift with each new quote.
    /// </para>
    /// <para>
    /// Performance optimization: Caches Slope results and only calculates new Slope values
    /// incrementally. Window application is O(n) but avoids O(n²) Slope recalculation.
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override (StdDevChannelsResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        int currentCount = ProviderCache.Count;

        // Incrementally calculate new Slope values
        if (_slopeCache.Count < currentCount)
        {
            for (int si = _slopeCache.Count; si < currentCount; si++)
            {
                SlopeResult slopeResult = CalculateSlope(ProviderCache, si, LookbackPeriods);
                _slopeCache.Add(slopeResult);
            }
        }

        // Recalculate channel assignments when provider count changes
        // Window boundaries shift, so all assignments must be recomputed
        if (currentCount != _lastSyncCount)
        {
            // Resize cache if needed
            while (Cache.Count < currentCount)
            {
                int idx = Cache.Count;
                Cache.Add(new StdDevChannelsResult(_slopeCache[idx].Timestamp));
            }

            // Reset all values to empty before reapplying windows
            for (int ri = 0; ri < currentCount; ri++)
            {
                Cache[ri] = new StdDevChannelsResult(_slopeCache[ri].Timestamp);
            }

            // Apply regression lines in reverse windows
            for (int wi = currentCount - 1; wi >= LookbackPeriods - 1; wi -= LookbackPeriods)
            {
                SlopeResult s = _slopeCache[wi];
                double? width = StandardDeviations * s.StdDev;

                // Assign values to all points in this window
                for (int p = wi - LookbackPeriods + 1; p <= wi; p++)
                {
                    if (p < 0) continue;

                    double? c = (s.Slope * (p + 1)) + s.Intercept;

                    Cache[p] = Cache[p] with
                    {
                        Centerline = c,
                        UpperChannel = c + width,
                        LowerChannel = c - width,
                        BreakPoint = p == wi - LookbackPeriods + 1
                    };
                }
            }

            _lastSyncCount = currentCount;
        }

        return (Cache[i], i);
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
