namespace Skender.Stock.Indicators;

// TODO: unsure if used

public abstract class AbstractQuoteInResultOut<TIn, TOut>
    : AbstractResultProvider<TOut>, IQuoteObserver<TIn>
    where TIn : struct, IQuote
    where TOut : struct, IResult
{
    internal AbstractQuoteInResultOut(
        IQuoteProvider<TIn> provider)
    {
        Provider = provider;
    }

    // reminder: these are the subscriber members only

    // PROPERTIES

    public IQuoteProvider<TIn> Provider { get; internal set; }

    public bool IsSubscribed => Subscription is not null;

    internal IDisposable? Subscription;

    protected IReadOnlyList<TIn> ProviderCache => Provider.Results;


    // METHODS

    public void OnNext((Act, TIn) value)
        => OnNextArrival(value.Item1, value.Item2);

    internal abstract void OnNextArrival(Act act, IQuote inbound);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => Subscription?.Dispose();

    // clear and resubscribe
    public void Reinitialize(bool withRebuild = true)
    {
        Unsubscribe();
        Subscription = Provider.Subscribe(this);

        if (withRebuild)
        {
            RebuildCache();
        }
        else
        {
            ClearCache();
        }
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
