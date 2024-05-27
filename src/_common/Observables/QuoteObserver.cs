namespace Skender.Stock.Indicators;

// QUOTE OBSERVER

public abstract class QuoteObserver<TQuote, TResult>
    : ResultCache<TResult>, IQuoteObserver<TResult>
    where TQuote : IQuote, new()
    where TResult : IResult, new()
{
    internal IDisposable? unsubscriber;

    internal QuoteObserver(
        QuoteProvider<TQuote> provider,
        bool isChainor) : base(isChainor)
    {
        QuoteSupplier = provider;
    }

    // PROPERTIES

    internal QuoteProvider<TQuote> QuoteSupplier { get; }

    // METHODS

    // standard observer properties

    public abstract void OnNext((Act act, IQuote quote) value);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => unsubscriber?.Dispose();

    // re/initialize my cache, from provider cache
    public void RebuildCache()
    {
        // nothing to do
        if (QuoteSupplier.Cache.Count == 0)
        {
            return;
        }

        // rebuild
        RebuildCache(0);
    }

    // replay from supplier cache, from date
    public void RebuildCache(DateTime fromTimestamp)
        => RebuildCache(fromTimestamp, 0);

    internal override void RebuildCache(
        DateTime fromTimestamp, int offset = 0)
    {
        int fromIndex = QuoteSupplier.Cache.FindIndex(fromTimestamp);

        if (fromIndex == -1)
        {
            throw new InvalidOperationException(
                "Cache rebuild starting target not found.");
        }

        RebuildCache(fromIndex, offset);
    }

    // replay from supplier cache, from index
    internal override void RebuildCache(
        int fromIndex, int offset = 0)
    {
        int firstIndex = fromIndex + offset;

        // clear forward values
        ClearCache(firstIndex);

        // replay from source
        for (int i = firstIndex; i < QuoteSupplier.Cache.Count; i++)
        {
            TQuote quote = QuoteSupplier.Cache[i];
            OnNext((Act.AddNew, quote));
        }
    }

    // delete cache range values (implemented in inheritor)
    internal abstract override void ClearCache(int fromIndex, int toIndex);
}
