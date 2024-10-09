namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAM HUB)

#region hub interface
public interface ISmaHub
{
    int LookbackPeriods { get; }
}
#endregion

public class SmaHub<TIn>
    : ChainProvider<TIn, SmaResult>, ISmaHub
    where TIn : IReusable
{
    #region constructors

    private readonly string hubName;

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

    public int LookbackPeriods { get; init; }

    // METHODS

    public override string ToString() => hubName;

    protected override (SmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.GetIndex(item, true);

        // candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: Sma.Increment(ProviderCache, LookbackPeriods, i).NaN2Null());

        return (r, i);
    }
}
