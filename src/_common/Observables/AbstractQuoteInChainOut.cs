namespace Skender.Stock.Indicators;

public abstract class AbstractQuoteInChainOut<TIn, TOut>
    : AbstractChainProvider<TOut>, IQuoteObserver<TIn>
    where TIn : struct, IQuote
    where TOut : struct, IReusableResult
{
    internal AbstractQuoteInChainOut(
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

    // rebuild cache from date
    public void RebuildCache(DateTime fromTimestamp)
        => RebuildCache(fromTimestamp, 0);

    // rebuild cache from timestamp
    private void RebuildCache(
        DateTime fromTimestamp, int offset = 0)
    {
        int fromIndex = Cache
            .FindIndex(fromTimestamp);

        if (fromIndex == -1)
        {
            throw new InvalidOperationException(
                "Cache rebuild starting date not found.");
        }

        RebuildCache(fromIndex, offset);
    }

    // rebuild cache from index
    private void RebuildCache(int fromIndex, int offset = 0)
    {
        ClearCache(fromIndex, offset);
        Provider.Resend(fromIndex, this);
    }
}