namespace Skender.Stock.Indicators;

public abstract class AbstractChainInResultOut<TIn, TOut>
    : AbstractResultProvider<TOut>, IChainObserver<TIn>
    where TIn : struct, IReusable
    where TOut : struct, IResult
{
    internal AbstractChainInResultOut(
        IChainProvider<TIn> provider)
    {
        Provider = provider;
    }

    // reminder: these are the subscriber members only

    // PROPERTIES

    public IChainProvider<TIn> Provider { get; internal set; }

    public bool IsSubscribed => Subscription is not null;

    internal IDisposable? Subscription;

    protected IReadOnlyList<TIn> ProviderCache => Provider.Results;


    // METHODS

    public void OnNext((Act, TIn) value)
        => OnNextArrival(value.Item1, value.Item2);

    internal abstract void OnNextArrival(Act act, IReusable inbound);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();

    // restart subscription
    public void Reinitialize()
    {
        Unsubscribe();
        ClearCache();

        Subscription = Provider.Subscribe(this);
    }

    // rebuild cache
    public void RebuildCache() => RebuildCache(0);

    /// <inheritdoc/>
    public void RebuildCache(
        DateTime fromTimestamp)
    {
        int fromIndex = Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

        if (fromIndex != -1)
        {
            RebuildCache(fromIndex);
        }
    }

    /// <inheritdoc/>
    public void RebuildCache(int fromIndex)
        => RebuildCache(fromIndex, toIndex: Cache.Count - 1);

    // rebuild cache from index
    protected override void RebuildCache(
        int fromIndex, int? toIndex = null)
    {
        ClearCache(fromIndex, toIndex);
        Provider.Resend(this, fromIndex, toIndex);
    }
}
