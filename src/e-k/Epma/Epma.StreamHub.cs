namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Endpoint Moving Average (EPMA)
/// </summary>
public class EpmaHub
    : ChainHub<IReusable, EpmaResult>, IEpma
{
    internal EpmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Epma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"EPMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    protected override (EpmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result
        EpmaResult r = new(
            Timestamp: item.Timestamp,
            Epma: Epma.Increment(ProviderCache, LookbackPeriods, i).NaN2Null());

        return (r, i);
    }
}

public static partial class Epma
{
    /// <summary>
    /// Converts the chain provider to an EPMA hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An EPMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static EpmaHub ToEpmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);
}
