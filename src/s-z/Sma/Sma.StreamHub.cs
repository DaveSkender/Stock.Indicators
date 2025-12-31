namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Represents a Simple Moving Average (SMA) stream hub.
/// </summary>
public class SmaHub
    : ChainProvider<IReusable, SmaResult>, ISma
{

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    internal SmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"SMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    protected override (SmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate SMA efficiently using a rolling window over ProviderCache
        // This is O(lookbackPeriods) which is constant for a given configuration
        // and maintains exact precision with Series implementation
        double sma = Sma.Increment(ProviderCache, LookbackPeriods, i);

        // candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: sma.NaN2Null());

        return (r, i);
    }

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
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An SMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmaHub ToSmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);
}
