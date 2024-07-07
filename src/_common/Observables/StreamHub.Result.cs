
namespace Skender.Stock.Indicators;

// RESULT HUB (STREAMING)

/// <summary>
/// ResultHub is the base type for streaming indicators that
/// are not chainable.  It can be observed by external subscribers.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public abstract class ResultHub<TIn, TOut>
    : ResultProvider<TOut>, IObserverHub<TIn, TOut>
    where TIn : struct, ISeries
    where TOut : struct, IResult
{
    private protected ResultHub(
        StreamProvider<TIn> provider,
        StreamCache<TOut> cache) : base(cache)
    {
        Supplier = provider;
        Observer = new(this, cache, this, provider);
    }

    public StreamProvider<TIn> Supplier { get; }

    public StreamObserver<TIn, TOut> Observer { get; }

    // hub

    public abstract override string ToString();

    // cache

    public IReadOnlyList<TOut> Results
        => StreamCache.Cache;

    // observer

    public abstract void OnNextNew(TIn newItem);

    public void Reinitialize()
        => Observer.Reinitialize();

    public void Unsubscribe()
        => Observer.Unsubscribe();

    public void RebuildCache()
        => Observer.RebuildCache(0);

    public void RebuildCache(DateTime fromTimestamp)
        => Observer.RebuildCache(fromTimestamp);

    public void RebuildCache(int fromIndex)
        => Observer.RebuildCache(fromIndex);
}
