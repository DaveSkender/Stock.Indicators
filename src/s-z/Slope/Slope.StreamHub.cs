namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Slope and Linear Regression indicator in a streaming context.
/// </summary>
public class SlopeHub
    : ChainProvider<IReusable, SlopeResult>, ISlope
{
    private readonly string hubName;

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
        hubName = $"SLOPE({lookbackPeriods})";

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

        // Skip warmup period
        if (i < LookbackPeriods - 1)
        {
            return (new SlopeResult(item.Timestamp), i);
        }

        // Calculate sums from provider cache window
        double sumX = 0;
        double sumY = 0;

        for (int p = i - LookbackPeriods + 1; p <= i; p++)
        {
            IReusable ps = ProviderCache[p];
            sumX += p + 1d;
            sumY += ps.Value;
        }

        double avgX = sumX / LookbackPeriods;
        double avgY = sumY / LookbackPeriods;

        // Calculate deviations and products
        double sumSqX = 0;
        double sumSqY = 0;
        double sumSqXy = 0;

        for (int p = i - LookbackPeriods + 1; p <= i; p++)
        {
            IReusable ps = ProviderCache[p];

            double devX = p + 1d - avgX;
            double devY = ps.Value - avgY;

            sumSqX += devX * devX;
            sumSqY += devY * devY;
            sumSqXy += devX * devY;
        }

        // Calculate slope and intercept
        double? slope = (sumSqXy / sumSqX).NaN2Null();
        double? intercept = (avgY - (slope * avgX)).NaN2Null();

        // Calculate Standard Deviation and R-Squared
        double stdDevX = Math.Sqrt(sumSqX / LookbackPeriods);
        double stdDevY = Math.Sqrt(sumSqY / LookbackPeriods);

        double? rSquared = null;

        const double epsilon = 1e-10;
        if (Math.Abs(stdDevX * stdDevY) > epsilon)
        {
            double arrr = sumSqXy / (stdDevX * stdDevY) / LookbackPeriods;
            rSquared = (arrr * arrr).NaN2Null();
        }

        // Calculate Line value (y = mx + b)
        // Line is only calculated for the current position in streaming
        // This differs from Series which calculates for entire last window
        decimal? line = null;
        if (slope.HasValue && intercept.HasValue)
        {
            line = (decimal?)((slope * (i + 1)) + intercept).NaN2Null();
        }

        // Create result
        SlopeResult r = new(
            Timestamp: item.Timestamp,
            Slope: slope,
            Intercept: intercept,
            StdDev: stdDevY.NaN2Null(),
            RSquared: rSquared,
            Line: line);

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // No state to rollback - we calculate from ProviderCache each time
        // TODO: If future optimizations add incremental state (e.g., rolling windows, cached sums),
        //       this method must be updated to restore state from ProviderCache after provider history mutations.
        //       Reference AdxHub.RollbackState() for state restoration patterns.
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
