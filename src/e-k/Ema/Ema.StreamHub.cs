namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

#region hub interface

public interface IEmaHub
{
    int LookbackPeriods { get; }
    double K { get; }
}
#endregion

public class EmaHub<TIn>
    : ChainProvider<EmaResult>, IStreamHub<TIn, EmaResult>, IEmaHub
    where TIn : struct, IReusable
{
    private readonly StreamCache<EmaResult> _cache;
    private readonly StreamObserver<TIn, EmaResult> _observer;
    private readonly ChainProvider<TIn> _supplier;

    #region constructors

    public EmaHub(
        ChainProvider<TIn> provider,
        int lookbackPeriods)
        : this(provider, cache: new(), lookbackPeriods) { }

    private EmaHub(
        ChainProvider<TIn> provider,
        StreamCache<EmaResult> cache,
        int lookbackPeriods) : base(cache)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        _cache = cache;
        _supplier = provider;
        _observer = new(this, this, provider);
    }
    #endregion

    public int LookbackPeriods { get; }
    public double K { get; }


    // METHODS

    public override string ToString()
        => $"EMA({LookbackPeriods})";

    public void OnNextNew(TIn newItem)
    {
        double ema;

        int i = _supplier.Position(newItem);

        if (i >= LookbackPeriods - 1)
        {
            IReusable last = ReadCache[i - 1];

            ema = !double.IsNaN(last.Value)

                // normal
                ? Ema.Increment(K, last.Value, newItem.Value)

                // re/initialize
                : Sma.Increment(_supplier.ReadCache, i, LookbackPeriods);
        }

        // warmup periods are never calculable
        else
        {
            ema = double.NaN;
        }

        // candidate result
        EmaResult r = new(
            Timestamp: newItem.Timestamp,
            Ema: ema.NaN2Null());


        // save to cache
        Act act = _cache.Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }

    #region inherited methods

    public void Unsubscribe() => _observer.Unsubscribe();
    public void Reinitialize() => _observer.Reinitialize();
    public void RebuildCache() => _observer.RebuildCache();
    public void RebuildCache(DateTime fromTimestamp) => _observer.RebuildCache(fromTimestamp);
    public void RebuildCache(int fromIndex) => _observer.RebuildCache(fromIndex);
    #endregion
}
