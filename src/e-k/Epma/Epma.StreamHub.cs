namespace Skender.Stock.Indicators;

// ENDPOINT MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Provides methods for creating EPMA hubs.
/// </summary>
public static partial class Epma
{
    /// <summary>
    /// Converts the chain provider to an EPMA hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>An EPMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static EpmaHub<TIn> ToEpmaHub<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents an Endpoint Moving Average (EPMA) stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class EpmaHub<TIn>
    : ChainProvider<TIn, EpmaResult>, IEpma
    where TIn : IReusable
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="EpmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal EpmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Epma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"EPMA({lookbackPeriods})";

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
    protected override (EpmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result
        EpmaResult r = new(
            Timestamp: item.Timestamp,
            Epma: Epma.Increment(ProviderCache, LookbackPeriods, i).NaN2Null());

        return (r, i);
    }
}
