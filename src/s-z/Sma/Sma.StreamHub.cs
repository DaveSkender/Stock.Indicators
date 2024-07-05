namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

#region hub interface
public interface ISmaHub
{
    int LookbackPeriods { get; }
}
#endregion

public class SmaHub<TIn>
    : ChainProvider<SmaResult>, IStreamHub<TIn, SmaResult>, ISmaHub
    where TIn : struct, IReusable
{
    private readonly StreamCache<SmaResult> _cache;
    private readonly StreamObserver<TIn, SmaResult> _observer;
    private readonly ChainProvider<TIn> _supplier;

    #region constructors

    public SmaHub(
        ChainProvider<TIn> provider,
        int lookbackPeriods)
        : this(provider, cache: new(), lookbackPeriods) { }

    private SmaHub(
        ChainProvider<TIn> provider,
        StreamCache<SmaResult> cache,
        int lookbackPeriods) : base(cache)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _cache = cache;
        _supplier = provider;
        _observer = new(this, cache, this, provider);
    }
    #endregion

    public int LookbackPeriods { get; }


    // METHODS

    public override string ToString()
        => $"SMA({LookbackPeriods})";

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextNew(TIn newItem)
    {
        int i = _supplier.Position(newItem.Timestamp);

        // candidate result
        SmaResult r = new(
            Timestamp: newItem.Timestamp,
            Sma: Sma.Increment(_supplier.ReadCache, i, LookbackPeriods).NaN2Null());

        // save to cache
        Act act = _cache.Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }
}
