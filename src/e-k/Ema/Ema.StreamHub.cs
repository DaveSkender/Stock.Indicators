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

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextArrival(Act act, TIn inbound)
    {
        int i;
        EmaResult r;

        // handle deletes
        if (act is Act.Delete)
        {
            i = _cache.CacheX
                .FindIndex(c => c.Timestamp == inbound.Timestamp);

            // cache entry unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching cache entry not found.");
            }

            r = CacheP[i];
        }

        // calculate incremental value
        else
        {
            i = _supplier.CacheP
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
                ref readonly EmaResult last
                    = ref SpanCache[i - 1];

                // normal
                if (last.Ema is not null)
                {
                    ema = Ema.Increment(K, (double)last.Ema, inbound.Value);
                }

                // set first value (normal) or reset
                // when prior EMA was incalculable
                else
                {
                    double sum = 0;
                    for (int w = i - LookbackPeriods + 1; w <= i; w++)
                    {
                        ref readonly TIn item
                            = ref _supplier.SpanCache[w];

                        sum += item.Value;
                    }

                    ema = sum / LookbackPeriods;
                }
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
        if (act != Act.AddNew && i < _supplier.CacheP.Count - 1)
        {
            int next = act == Act.Delete ? i : i + 1;
            ref readonly TIn value = ref _supplier.SpanCache[next];
            OnNextArrival(Act.Update, value);
        }
    }
}
