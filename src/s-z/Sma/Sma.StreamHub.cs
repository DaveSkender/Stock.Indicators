namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAM HUB)

#region hub interface and initializer

/// <summary>
/// Interface for Simple Moving Average (SMA) hub.
/// </summary>
public interface ISma
{
    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    int LookbackPeriods { get; }
}

/// <summary>
/// Provides methods for creating SMA hubs.
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Converts the chain provider to an SMA hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>An SMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmaHub<TIn> ToSma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods);
}
#endregion

/// <summary>
/// Represents a Simple Moving Average (SMA) stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class SmaHub<TIn>
    : ChainProvider<TIn, SmaResult>, ISma
    where TIn : IReusable
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal SmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"SMA({lookbackPeriods})";

        Reinitialize();
    }
    #endregion

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: Sma.Increment(ProviderCache, LookbackPeriods, i).NaN2Null());

        return (r, i);
    }
}
