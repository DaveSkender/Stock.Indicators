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
        _observer = new(this, cache, this, provider);
    }
    #endregion

    public int LookbackPeriods { get; }
    public double K { get; }


    // METHODS

    public override string ToString()
        => $"EMA({LookbackPeriods})";

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextArrival(Act act, TIn inbound)
    {
        int i;
        EmaResult r;

        // handle deletes
        if (act is Act.Delete)
        {
            i = _cache.Cache
                .FindIndex(c => c.Timestamp == inbound.Timestamp);

            // cache entry unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching cache entry not found.");
            }

            r = Cache[i];
        }

        // calculate incremental value
        else
        {
            i = _supplier.Cache
                .FindIndex(c => c.Timestamp == inbound.Timestamp);

            // source unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching source history not found.");
            }

            // normal
            double ema;

            if (i >= LookbackPeriods - 1)
            {
                IReusable last = ReadCache[i - 1];

                ema = !double.IsNaN(last.Value)

                    // normal
                    ? Ema.Increment(K, last.Value, inbound.Value)

                    // re/initialize
                    : Sma.Increment(_supplier.ReadCache, i, LookbackPeriods);
            }

            // warmup periods are never calculable
            else
            {
                ema = double.NaN;
            }

            // candidate result
            r = new(
                Timestamp: inbound.Timestamp,
                Ema: ema.NaN2Null());
        }

        // save to cache
        act = _cache.Modify(act, r);

        // send to observers
        NotifyObservers(act, r);

        // cascade update forward values (recursively)
        // TODO: optimize this
        if (act != Act.AddNew && i < _supplier.Cache.Count - 1)
        {
            int next = act == Act.Delete ? i : i + 1;
            OnNextArrival(Act.Update, _supplier.ReadCache[next]);
        }
    }
}
