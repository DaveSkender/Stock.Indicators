namespace Skender.Stock.Indicators;

// TODO: unsure if used, needed
// HeikinAshi and Renko are still IQuote struct types? (probably still need to emit IQuoteProvider)

public abstract class AbstractQuoteInQuoteOut<TIn, TOut>
    : AbstractQuoteProvider<TOut>, IQuoteObserver<TIn>
    where TIn : struct, IQuote
    where TOut : struct, IQuote
{
    // observer members only

    private protected AbstractQuoteInQuoteOut(
        IQuoteProvider<TIn> provider)
    {
        Provider = provider;
    }

    public IQuoteProvider<TIn> Provider { get; }

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
