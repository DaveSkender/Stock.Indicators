namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAM HUB)

#region hub interface
public interface ISmaHub
{
    int LookbackPeriods { get; }
}
#endregion

public class SmaHub<TIn>
    : ChainHub<TIn, SmaResult>, ISmaHub
    where TIn : struct, IReusable
{
    #region constructors

    public SmaHub(
        ChainProvider<TIn> provider,
        int lookbackPeriods)
        : this(provider, cache: new(), lookbackPeriods) { }

    private SmaHub(
        ChainProvider<TIn> provider,
        StreamCache<SmaResult> cache,
        int lookbackPeriods) : base(provider, cache)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; }

    // METHODS

    public override string ToString()
        => $"SMA({LookbackPeriods})";

    public override void OnNextNew(TIn newItem)
    {
        int i = Supplier.StreamCache.Position(newItem);

        // candidate result
        SmaResult r = new(
            Timestamp: newItem.Timestamp,
            Sma: Sma.Increment(Supplier.StreamCache.ReadCache, i, LookbackPeriods).NaN2Null());

        // save to cache
        Act act = StreamCache.Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }
}
