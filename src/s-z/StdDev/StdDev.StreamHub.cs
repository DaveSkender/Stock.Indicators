namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (STREAM HUB)

/// <summary>
/// Represents a Standard Deviation stream hub.
/// </summary>
public class StdDevHub
    : ChainProvider<IReusable, StdDevResult>, IStdDev
{

    /// <summary>
    /// Initializes a new instance of the <see cref="StdDevHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    internal StdDevHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        StdDev.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"STDDEV({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <inheritdoc/>
    protected override (StdDevResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate StdDev using two-pass algorithm over ProviderCache
        // This is O(lookbackPeriods) complexity (linear in lookback period)
        // Two-pass method is necessary for numerical stability and exact Series precision
        double? stdDev = null;
        double? mean = null;
        double? zScore = null;

        if (i >= LookbackPeriods - 1)
        {
            // Calculate mean using Sma.Increment utility
            double meanValue = Sma.Increment(ProviderCache, LookbackPeriods, i);

            if (!double.IsNaN(meanValue))
            {
                mean = meanValue;

                // Calculate sum of squared deviations (numerically stable method)
                double sumSqDev = 0;
                for (int p = i - LookbackPeriods + 1; p <= i; p++)
                {
                    double value = ProviderCache[p].Value;
                    double deviation = value - mean.Value;
                    sumSqDev += deviation * deviation;
                }

                // Calculate standard deviation
                stdDev = Math.Sqrt(sumSqDev / LookbackPeriods);

                // Calculate z-score
                zScore = stdDev == 0 ? double.NaN : (item.Value - mean.Value) / stdDev.Value;
            }
        }

        // candidate result
        StdDevResult r = new(
            Timestamp: item.Timestamp,
            StdDev: stdDev,
            Mean: mean,
            ZScore: zScore.NaN2Null());

        return (r, i);
    }

}

/// <summary>
/// Provides methods for creating StdDev hubs.
/// </summary>
public static partial class StdDev
{
    /// <summary>
    /// Converts the chain provider to a StdDev hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A StdDev hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static StdDevHub ToStdDevHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a StdDev hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="StdDevHub"/>.</returns>
    public static StdDevHub ToStdDevHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStdDevHub(lookbackPeriods);
    }
}
