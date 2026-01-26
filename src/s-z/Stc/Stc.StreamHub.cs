namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for creating Schaff Trend Cycle (STC) streaming hubs.
/// </summary>
public class StcHub
    : ChainHub<MacdResult, StcResult>, IStc
{
    private readonly RollingWindowMax<double> _macdHighWindow;
    private readonly RollingWindowMin<double> _macdLowWindow;
    private readonly Queue<double> _rawKBuffer;

    internal StcHub(
        IChainProvider<IReusable> provider,
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
        : this(
            provider.ToMacdHub(fastPeriods, slowPeriods, 0),
            cyclePeriods)
    { }

    internal StcHub(
        MacdHub macdHub,
        int cyclePeriods)
        : base(macdHub)
    {
        ArgumentNullException.ThrowIfNull(macdHub);
        Stc.Validate(cyclePeriods, macdHub.FastPeriods, macdHub.SlowPeriods);

        CyclePeriods = cyclePeriods;
        FastPeriods = macdHub.FastPeriods;
        SlowPeriods = macdHub.SlowPeriods;

        // Stochastic rolling windows for MACD values
        _macdHighWindow = new RollingWindowMax<double>(cyclePeriods);
        _macdLowWindow = new RollingWindowMin<double>(cyclePeriods);

        // Buffer for raw %K smoothing (smoothPeriods = 3)
        _rawKBuffer = new Queue<double>(3);

        Name = $"STC({cyclePeriods},{FastPeriods},{SlowPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int CyclePeriods { get; init; }

    /// <inheritdoc/>
    public int FastPeriods { get; init; }

    /// <inheritdoc/>
    public int SlowPeriods { get; init; }

    /// <inheritdoc/>
    protected override (StcResult result, int index)
        ToIndicator(MacdResult item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Get MACD value from chained input
        double macd = item.Macd ?? double.NaN;

        // Add MACD to rolling windows for stochastic calculation (only if not NaN)
        if (!double.IsNaN(macd))
        {
            _macdHighWindow.Add(macd);
            _macdLowWindow.Add(macd);
        }

        // Calculate raw %K from MACD values
        double rawK = double.NaN;
        if (i >= SlowPeriods + CyclePeriods - 2 &&
            !double.IsNaN(macd) &&
            _macdHighWindow.Count >= CyclePeriods &&
            _macdLowWindow.Count >= CyclePeriods)
        {
            double highHigh = _macdHighWindow.GetMax();
            double lowLow = _macdLowWindow.GetMin();

            rawK = (highHigh - lowLow) != 0
                 ? 100 * (macd - lowLow) / (highHigh - lowLow)
                 : 0;
        }

        // Buffer raw %K for smoothing (match Stoch.StreamHub pattern: always enqueue, even NaN)
        _rawKBuffer.Enqueue(rawK);
        if (_rawKBuffer.Count > 3)
        {
            _rawKBuffer.Dequeue();
        }

        // Calculate smoothed %K (STC value) using SMA with smoothPeriods=3
        // Match Stoch.StreamHub pattern: check index >= smoothPeriods before calculating
        // Natural NaN propagation through summation
        double? stc = null;
        if (i >= SlowPeriods + CyclePeriods)
        {
            double sum = 0;
            foreach (double rawKValue in _rawKBuffer)
            {
                sum += rawKValue;  // NaN propagates naturally
            }

            double smoothedK = sum / 3;
            stc = double.IsNaN(smoothedK) ? null : smoothedK;
        }

        StcResult result = new(
            Timestamp: item.Timestamp,
            Stc: stc);

        return (result, i);
    }

    /// <summary>
    /// Restores the stochastic state up to the specified timestamp.
    /// MACD state is handled automatically by the chained MacdHub.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear rolling windows and buffer
        _macdHighWindow.Clear();
        _macdLowWindow.Clear();
        _rawKBuffer.Clear();

        // Rebuild from MacdHub results up to the rollback point
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;

        // Rebuild MACD rolling windows for cycle periods
        int startIdx = Math.Max(0, targetIndex + 1 - CyclePeriods);
        for (int p = startIdx; p <= targetIndex; p++)
        {
            MacdResult macdResult = ProviderCache[p];
            double macdValue = macdResult.Macd ?? double.NaN;
            if (!double.IsNaN(macdValue))
            {
                _macdHighWindow.Add(macdValue);
                _macdLowWindow.Add(macdValue);
            }
        }

        // Prefill raw %K buffer (smoothPeriods = 3)
        if (targetIndex >= SlowPeriods + CyclePeriods - 2)
        {
            int kStart = Math.Max(SlowPeriods + CyclePeriods - 2, targetIndex + 1 - 3);
            for (int p = kStart; p <= targetIndex; p++)
            {
                // Calculate raw %K for this position using MacdHub results
                int rStart = Math.Max(0, p + 1 - CyclePeriods);
                double hh = double.NegativeInfinity;
                double ll = double.PositiveInfinity;

                for (int r = rStart; r <= p; r++)
                {
                    MacdResult macdAtR = ProviderCache[r];
                    double macdValue = macdAtR.Macd ?? double.NaN;
                    if (macdValue > hh)
                    {
                        hh = macdValue;
                    }

                    if (macdValue < ll)
                    {
                        ll = macdValue;
                    }
                }

                MacdResult macdAtP = ProviderCache[p];
                double macdAtPValue = macdAtP.Macd ?? double.NaN;
                double rawAtP = (hh - ll) != 0 ? 100 * (macdAtPValue - ll) / (hh - ll) : 0;
                _rawKBuffer.Enqueue(rawAtP);
            }
        }
    }
}

public static partial class Stc
{
    /// <summary>
    /// Creates a Schaff Trend Cycle (STC) streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle. Default is 10.</param>
    /// <param name="fastPeriods">The number of fast periods for the MACD calculation. Default is 23.</param>
    /// <param name="slowPeriods">The number of slow periods for the MACD calculation. Default is 50.</param>
    /// <returns>A STC hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static StcHub ToStcHub(
        this IChainProvider<IReusable> chainProvider,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        => new(chainProvider, cyclePeriods, fastPeriods, slowPeriods);

    /// <summary>
    /// Creates a new STC hub, using values from an existing MACD hub.
    /// </summary>
    /// <param name="macdHub">The MACD hub.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle. Default is 10.</param>
    /// <returns>An STC hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the MACD hub is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when cyclePeriods is invalid.</exception>
    /// <remarks>
    /// <para>IMPORTANT: This is not a normal chaining approach.</para>
    /// This extension overrides and enables a chain that specifically
    /// reuses the existing <see cref="MacdHub"/> in its internal construction.
    ///</remarks>
    public static StcHub ToStcHub(
        this MacdHub macdHub,
        int cyclePeriods = 10)
        => new(macdHub, cyclePeriods);
}
