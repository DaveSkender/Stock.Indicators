namespace Skender.Stock.Indicators;

public abstract class AbstractChainInChainOut<TIn, TOut>
    : AbstractChainProvider<TOut>, IChainObserver<TIn>
    where TIn : struct, IReusable
    where TOut : struct, IReusable
{
    // observer members only

    internal AbstractChainInChainOut(
        IChainProvider<TIn> provider)
    {
        Provider = provider;
    }

    public IChainProvider<TIn> Provider { get; }

    public bool IsSubscribed => Subscription is not null;

    protected IDisposable? Subscription { get; set; }

    protected IReadOnlyList<TIn> ProviderCache => Provider.Results;


    # region METHODS (OBSERVER)

    public void OnNext((Act, TIn) value)
        => OnNextArrival(value.Item1, value.Item2);

    protected abstract void OnNextArrival(Act act, TIn inbound);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();
    #endregion

    #region METHODS (CACHE UTILITIES)

    /// full reset
    /// <inheritdoc/>
    public void Reinitialize()
    {
        Unsubscribe();
        ClearCache();
        Subscription = Provider.Subscribe(this);
    }

    /// build cache from timestamp
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

    /// rebuild cache from index
    /// <inheritdoc/>
    public void RebuildCache(int fromIndex)
    {
        ClearCache(fromIndex);
        Provider.Resend(this, fromIndex);
    }
    #endregion
}
