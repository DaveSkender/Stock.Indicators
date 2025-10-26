namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Provides methods for creating WMA hubs.
/// </summary>
public class WmaHub
    : ChainProvider<IReusable, WmaResult>, IWma
{
    private readonly string hubName;
    private readonly Queue<double> window;
    private readonly double divisor;
    private double weightedSum;

    /// <summary>
    /// Initializes a new instance of the <see cref="WmaHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal WmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Wma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"WMA({lookbackPeriods})";
        window = new Queue<double>(lookbackPeriods);
        divisor = (double)LookbackPeriods * (LookbackPeriods + 1) / 2d;
        weightedSum = 0;

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // METHODS

    /// <inheritdoc />
    public override string ToString() => hubName;

    /// <inheritdoc />
    protected override (WmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

        // Optimized sliding window approach:
        // - Uses Queue for O(1) window management
        // - Converts to array for O(1) indexed access during weighted sum calculation
        // - Divisor pre-calculated in constructor
        double value = item.Value;
        double? wma = null;

        // Handle NaN values
        if (double.IsNaN(value))
        {
            // Reset state when encountering NaN
            window.Clear();
            weightedSum = 0;
        }
        else
        {
            // For WMA, we need to recalculate when window changes
            // Add new value to window
            window.Enqueue(value);

            // Remove oldest value if window is full
            if (window.Count > LookbackPeriods)
            {
                window.Dequeue();
            }

            // Calculate WMA when window is full
            if (window.Count == LookbackPeriods)
            {
                // WMA requires accessing all values with their weights
                // Convert queue to array for indexed access
                double[] windowArray = window.ToArray();
                weightedSum = 0;

                for (int i = 0; i < LookbackPeriods; i++)
                {
                    int weight = i + 1;
                    weightedSum += windowArray[i] * weight / divisor;
                }

                wma = weightedSum;
            }
        }

        WmaResult result = new(
            Timestamp: item.Timestamp,
            Wma: wma);

        return (result, index);
    }

    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        window.Clear();
        weightedSum = 0;

        // Rebuild window from ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        for (int p = startIdx; p <= targetIndex; p++)
        {
            double value = ProviderCache[p].Value;
            if (!double.IsNaN(value))
            {
                window.Enqueue(value);
            }
            else
            {
                // Reset on NaN
                window.Clear();
            }
        }
    }
}


public static partial class Wma
{
    /// <summary>
    /// Converts the chain provider to a WMA hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A WMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static WmaHub ToWmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
    {
        ArgumentNullException.ThrowIfNull(chainProvider);
        return new(chainProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates a Wma hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="WmaHub"/>.</returns>
    public static WmaHub ToWmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWmaHub(lookbackPeriods);
    }

}
