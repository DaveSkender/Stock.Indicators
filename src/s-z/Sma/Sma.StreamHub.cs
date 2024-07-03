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
        _observer = new(this, this, provider);
    }
    #endregion

    public int LookbackPeriods { get; }


    // METHODS

    public override string ToString()
        => $"SMA({LookbackPeriods})";

    public void Unsubscribe() => _observer.Unsubscribe();

    public void OnNextArrival(Act act, TIn inbound)
    {
        int i;
        SmaResult r;

        // handle deletes
        if (act == Act.Delete)
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
            double sma;

            if (i >= LookbackPeriods - 1)
            {
                double sum = 0;
                for (int w = i - LookbackPeriods + 1; w <= i; w++)
                {
                    ref readonly TIn item
                        = ref _supplier.SpanCache[w];

                    sum += item.Value;
                }

                sma = sum / LookbackPeriods;
            }

            // warmup periods are never calculable
            else
            {
                sma = double.NaN;
            }

            // candidate result
            r = new(
                Timestamp: inbound.Timestamp,
                Sma: sma.NaN2Null());
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
