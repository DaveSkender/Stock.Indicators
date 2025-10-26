namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Represents a Simple Moving Average (SMA) stream hub.
/// </summary>
public class SmaHub
    : ChainProvider<IReusable, SmaResult>, ISma
{
    #region fields

    private readonly string hubName;
    private readonly Queue<double> window;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal SmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"SMA({lookbackPeriods})";
        window = new Queue<double>(lookbackPeriods);

        Reinitialize();
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    #endregion

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Optimized sliding window approach:
        // - Uses Queue for O(1) enqueue/dequeue operations
        // - Avoids repeated ProviderCache access (reduces infrastructure overhead)
        // - Recalculates sum from queue to maintain floating-point precision parity with Series
        double value = item.Value;
        double? sma = null;

        // Handle NaN values
        if (double.IsNaN(value))
        {
            // Reset state when encountering NaN
            window.Clear();
        }
        else
        {
            // Add new value to window
            window.Enqueue(value);

            // Remove oldest value if window is full
            if (window.Count > LookbackPeriods)
            {
                window.Dequeue();
            }

            // Calculate SMA when window is full
            // Sum from queue to match Series precision
            if (window.Count == LookbackPeriods)
            {
                double sum = 0;
                foreach (double val in window)
                {
                    sum += val;
                }
                sma = sum / LookbackPeriods;
            }
        }

        // candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: sma);

        return (r, i);
    }

    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        window.Clear();

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

    #endregion
}

/// <summary>
/// Provides methods for creating SMA hubs.
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Converts the chain provider to an SMA hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>An SMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmaHub ToSmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates an SMA hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>An instance of <see cref="SmaHub"/>.</returns>
    public static SmaHub ToSmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmaHub(lookbackPeriods);
    }
}
