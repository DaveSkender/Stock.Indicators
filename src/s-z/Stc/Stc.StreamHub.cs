namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Schaff Trend Cycle (STC) indicator.
/// </summary>
public sealed class StcHub
    : ChainProvider<IReusable, StcResult>
{
    private const int SmoothPeriods = 3; // Fixed smoothing period for STC calculation

    private readonly string hubName;

    /// <summary>
    /// Internal MACD hub for incremental MACD calculation
    /// </summary>
    private readonly MacdHub macdHub;

    /// <summary>
    /// Rolling windows for O(1) MACD max/min tracking
    /// </summary>
    private readonly RollingWindowMax<double> _macdMaxWindow;
    private readonly RollingWindowMin<double> _macdMinWindow;

    /// <summary>
    /// Rolling window for raw %K smoothing
    /// </summary>
    private readonly Queue<double> rawKBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StcHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle.</param>
    /// <param name="fastPeriods">The number of fast periods for the MACD calculation.</param>
    /// <param name="slowPeriods">The number of slow periods for the MACD calculation.</param>
    internal StcHub(
        IChainProvider<IReusable> provider,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50) : base(provider)
    {
        Stc.Validate(cyclePeriods, fastPeriods, slowPeriods);

        CyclePeriods = cyclePeriods;
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;

        hubName = $"STC({cyclePeriods},{fastPeriods},{slowPeriods})";

        // Create internal MACD hub for incremental MACD calculation
        macdHub = provider.ToMacdHub(fastPeriods, slowPeriods, 1);

        // Rolling windows for O(1) MACD max/min tracking
        _macdMaxWindow = new RollingWindowMax<double>(cyclePeriods);
        _macdMinWindow = new RollingWindowMin<double>(cyclePeriods);

        // Buffer for raw %K smoothing
        rawKBuffer = new Queue<double>(SmoothPeriods);

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods for the cycle.
    /// </summary>
    public int CyclePeriods { get; init; }

    /// <summary>
    /// Gets the number of fast periods for the MACD calculation.
    /// </summary>
    public int FastPeriods { get; init; }

    /// <summary>
    /// Gets the number of slow periods for the MACD calculation.
    /// </summary>
    public int SlowPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (StcResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? stc = null;

        // Get MACD value from the internal hub
        MacdResult? macdResult = macdHub.Results.ElementAtOrDefault(i);
        double? macdValue = macdResult?.Macd;

        // Only process if we have a valid MACD value
        if (macdValue.HasValue)
        {
            double? oscillator = UpdateStochasticState(macdValue.Value);

            if (oscillator.HasValue)
            {
                stc = oscillator;
            }
        }

        // candidate result
        StcResult result = new(
            Timestamp: item.Timestamp,
            Stc: stc);

        return (result, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int providerIndex = ProviderCache.IndexGte(timestamp);
        if (providerIndex == -1)
        {
            providerIndex = ProviderCache.Count;
        }

        // Rebuild underlying MACD hub so replay uses fresh MACD values
        macdHub.Rebuild(timestamp);

        // Reset state and replay historical MACD values up to the rebuild index
        _macdMaxWindow.Clear();
        _macdMinWindow.Clear();
        rawKBuffer.Clear();

        if (providerIndex <= 0)
        {
            return;
        }

        IReadOnlyList<MacdResult> macdResults = macdHub.Results;
        int replayLimit = Math.Min(providerIndex, macdResults.Count);

        for (int i = 0; i < replayLimit; i++)
        {
            MacdResult historical = macdResults[i];
            if (historical.Macd.HasValue)
            {
                _ = UpdateStochasticState(historical.Macd.Value);
            }
        }
    }

    private double? UpdateStochasticState(double macdValue)
    {
        // Add MACD value to rolling windows
        _macdMaxWindow.Add(macdValue);
        _macdMinWindow.Add(macdValue);

        if (_macdMaxWindow.Count != CyclePeriods)
        {
            return null;
        }

        // Get high/low MACD from rolling windows (O(1))
        double highMacd = _macdMaxWindow.Max;
        double lowMacd = _macdMinWindow.Min;

        // Calculate raw %K oscillator
        double rawK = Math.Abs(highMacd - lowMacd) > double.Epsilon
            ? 100d * (macdValue - lowMacd) / (highMacd - lowMacd)
            : 0d;

        // Add raw K to buffer for smoothing
        rawKBuffer.Enqueue(rawK);
        if (rawKBuffer.Count > SmoothPeriods)
        {
            _ = rawKBuffer.Dequeue();
        }

        if (rawKBuffer.Count != SmoothPeriods)
        {
            return null;
        }

        // Calculate smoothed %K (SMA of raw K values)
        double sumK = 0d;
        foreach (double item in rawKBuffer)
        {
            sumK += item;
        }

        double smoothedK = sumK / SmoothPeriods;

        // Signal line (signalPeriods = 1, so just return smoothedK)
        // Based on the series implementation which uses signalPeriods=1
        return smoothedK;
    }
}

public static partial class Stc
{
    /// <summary>
    /// Converts the chain provider to a Schaff Trend Cycle (STC) hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle.</param>
    /// <param name="fastPeriods">The number of fast periods for the MACD calculation.</param>
    /// <param name="slowPeriods">The number of slow periods for the MACD calculation.</param>
    /// <returns>A Schaff Trend Cycle hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static StcHub ToStcHub(
        this IChainProvider<IReusable> chainProvider,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        => new(chainProvider, cyclePeriods, fastPeriods, slowPeriods);

    /// <summary>
    /// Creates a Stc hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle.</param>
    /// <param name="fastPeriods">The number of fast periods for the MACD calculation.</param>
    /// <param name="slowPeriods">The number of slow periods for the MACD calculation.</param>
    /// <returns>An instance of <see cref="StcHub"/>.</returns>
    public static StcHub ToStcHub(
        this IReadOnlyList<IQuote> quotes,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStcHub(cyclePeriods, fastPeriods, slowPeriods);
    }
}
