namespace Skender.Stock.Indicators;

public abstract class AbstractChainInChainOut<TIn, TOut>
    : AbstractChainProvider<TOut>, IChainObserver<TIn>
    where TIn : struct, IReusable
    where TOut : struct, IReusable
{
    // observer members only

    #region CONSTRUCTORS

    internal AbstractChainInChainOut(
        IChainProvider<TIn> provider)
    {
        Provider = provider;
    }
    #endregion

    #region PROPERTIES

    public IChainProvider<TIn> Provider { get; private set; }

    public bool IsSubscribed => Subscription is not null;

    protected IDisposable? Subscription { get; set; }

    protected IReadOnlyList<TIn> ProviderCache => Provider.Results;
    #endregion

    # region METHODS (OBSERVER)

    public void OnNext((Act, TIn) value)
        => OnNextArrival(value.Item1, value.Item2);

    protected abstract void OnNextArrival(Act act, TIn inbound);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();
    #endregion

    #region METHODS (UTILITIES)

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
    #endregion
}
