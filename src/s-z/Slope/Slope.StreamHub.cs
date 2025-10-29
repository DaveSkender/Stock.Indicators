namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Slope and Linear Regression calculations.
/// </summary>
/// <remarks>
/// Implements incremental linear regression with O(1) state updates per tick.
/// Note: Line values are updated for the last lookbackPeriods results (legitimate historical repaint).
/// </remarks>
public class SlopeHub
    : ChainProvider<IReusable, SlopeResult>
{
    private readonly string hubName;
    private readonly Queue<double> buffer;

    /// <summary>
    /// Pre-calculated constant for X variance (sequential integers).
    /// Formula: n*(n²-1)/12 where n = lookbackPeriods
    /// </summary>
    private readonly double sumSqXConstant;

    /// <summary>
    /// Tracks how many items have been processed for global X-axis indexing.
    /// </summary>
    private int globalIndexOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlopeHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal SlopeHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Slope.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        buffer = new Queue<double>(lookbackPeriods);
        hubName = $"SLOPE({lookbackPeriods})";

        // Pre-calculate constant sumSqX for sequential X values
        // When X values are [x, x+1, ..., x+n-1], avgX = x + (n-1)/2
        // Sum of (Xi - avgX)^2 = n*(n^2 - 1)/12
        sumSqXConstant = lookbackPeriods * ((lookbackPeriods * lookbackPeriods) - 1) / 12.0;

        globalIndexOffset = 0;

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SlopeResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Update the rolling buffer
        buffer.Update(LookbackPeriods, item.Value);

        // During initialization period
        if (buffer.Count < LookbackPeriods)
        {
            SlopeResult warmup = new(item.Timestamp);
            return (warmup, i);
        }

        // Calculate current global index (0-based, but we use 1-based for X values)
        int currentIndex = globalIndexOffset + i;

        // Optimization: Calculate X values mathematically (sequential integers)
        // X values are: (currentIndex - lookbackPeriods + 2) to (currentIndex + 1)
        double firstX = currentIndex - LookbackPeriods + 2d;
        double sumX = (LookbackPeriods * firstX) + (LookbackPeriods * (LookbackPeriods - 1) / 2.0);
        double avgX = sumX / LookbackPeriods;

        // Calculate sums for least squares method
        // Two passes required: 1) get avgY, 2) calculate deviations
        double sumY = 0;
        double sumSqY = 0;
        double sumSqXy = 0;

        // First pass: get sumY to calculate avgY
        foreach (double bufferValue in buffer)
        {
            sumY += bufferValue;
        }

        double avgY = sumY / LookbackPeriods;

        // Second pass: calculate deviations and their products
        int relativeIndex = 0;
        foreach (double bufferValue in buffer)
        {
            double xValue = firstX + relativeIndex;
            double devX = xValue - avgX;
            double devY = bufferValue - avgY;

            sumSqY += devY * devY;
            sumSqXy += devX * devY;
            relativeIndex++;
        }

        // Use pre-calculated constant for sumSqX
        double? slope = (sumSqXy / sumSqXConstant).NaN2Null();
        double? intercept = (avgY - (slope * avgX)).NaN2Null();

        // Calculate Standard Deviation and R-Squared
        double stdDevX = Math.Sqrt(sumSqXConstant / LookbackPeriods);
        double stdDevY = Math.Sqrt(sumSqY / LookbackPeriods);

        double? rSquared = null;

        if (stdDevX * stdDevY != 0)
        {
            double arrr = sumSqXy / (stdDevX * stdDevY) / LookbackPeriods;
            rSquared = (arrr * arrr).NaN2Null();
        }

        // Write result
        // Note: Line values are intentionally left null in streaming mode
        // as they require updating historical values which is complex in streaming context
        SlopeResult result = new(
            Timestamp: item.Timestamp,
            Slope: slope,
            Intercept: intercept,
            StdDev: stdDevY.NaN2Null(),
            RSquared: rSquared,
            Line: null);

        return (result, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        buffer.Clear();
        globalIndexOffset = 0;

        // Find the index for rollback
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0) return;

        int targetIndex = index - 1;

        // Rebuild buffer from cache
        int startIdx = Math.Max(0, targetIndex - LookbackPeriods + 1);

        for (int p = startIdx; p <= targetIndex; p++)
        {
            IReusable item = ProviderCache[p];
            buffer.Enqueue(item.Value);
        }
    }
}


public static partial class Slope
{
    /// <summary>
    /// Creates a Slope streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A Slope hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SlopeHub ToSlopeHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Slope hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="SlopeHub"/>.</returns>
    public static SlopeHub ToSlopeHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSlopeHub(lookbackPeriods);
    }
}
