namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Provides methods for creating WMA hubs.
/// </summary>
public static partial class Wma
{
    /// <summary>
    /// Converts the chain provider to a WMA hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A WMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static WmaHub<TIn> ToWma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : IReusable
    {
        ArgumentNullException.ThrowIfNull(chainProvider);
        return new(chainProvider, lookbackPeriods);
    }
}

/// <summary>
/// Represents a Weighted Moving Average (WMA) stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class WmaHub<TIn>
    : ChainProvider<TIn, WmaResult>, IWma
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="WmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal WmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Wma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"WMA({lookbackPeriods})";

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
        ToIndicator(TIn item, int? indexHint)
    {
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

        double wma = Wma.Increment(ProviderCache, LookbackPeriods, index);

        WmaResult result = new(
            Timestamp: item.Timestamp,
            Wma: wma.NaN2Null());

        return (result, index);
    }
}
