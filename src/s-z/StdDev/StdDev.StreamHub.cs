namespace Skender.Stock.Indicators;

// STANDARD DEVIATION (STREAM HUB)

/// <summary>
/// Represents a Standard Deviation (StdDev) stream hub.
/// </summary>
public class StdDevHub
    : ChainProvider<IReusable, StdDevResult>, IStdDev
{
    #region fields

    private readonly string hubName;

    #endregion fields

    #region constructors

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
        hubName = $"STDDEV({lookbackPeriods})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (StdDevResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate StdDev efficiently using a rolling window over ProviderCache
        // This is O(lookbackPeriods) which is constant for a given configuration
        // and maintains exact precision with Series implementation
        double? stdDev = null;
        double? mean = null;
        double? zScore = null;

        if (i >= LookbackPeriods - 1)
        {
            double[] values = new double[LookbackPeriods];
            double sum = 0;
            int n = 0;
            bool hasNaN = false;

            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                IReusable ps = ProviderCache[p];
                double value = ps.Value;

                if (double.IsNaN(value))
                {
                    hasNaN = true;
                    break;
                }

                values[n] = value;
                sum += value;
                n++;
            }

            if (!hasNaN)
            {
                mean = sum / LookbackPeriods;

                // Use the same StdDev extension method as Series for consistency
                double stdDevValue = values.StdDev();
                stdDev = stdDevValue;

                zScore = stdDevValue == 0 ? double.NaN
                    : (item.Value - mean.Value) / stdDevValue;
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

    #endregion methods
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
