namespace Skender.Stock.Indicators;

// QUOTE OBSERVER

public abstract class QuoteObserver<TQuote, TResult>
    : SeriesCache<TResult>, IQuoteObserver<TQuote, TResult>
    where TQuote : IQuote, new()
    where TResult : IResult, new()
{
    internal IDisposable? unsubscriber;

    private protected QuoteObserver(QuoteProvider<TQuote> provider)
    {
        QuoteSupplier = provider;
    }

    // PROPERTIES

    internal QuoteProvider<TQuote> QuoteSupplier { get; }

    // METHODS

    // standard observer properties

    public abstract void OnNext((Act act, TQuote quote) value);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => unsubscriber?.Dispose();

    // re/initialize my cache, from provider cache
    public void Initialize()
    {
        ClearCache();  // delete cache entries

        // nothing to do
        if (QuoteSupplier.Cache.Count == 0)
        {
            return;
        }

        // rebuild from date
        DateTime fromDate = QuoteSupplier.Cache[0].TickDate;
        RebuildCache(fromDate);
    }

    // replay from supplier cache
    internal void RebuildCache(DateTime fromDate)
    {
        int startIndex = QuoteSupplier.Cache.FindIndex(fromDate);

        if (startIndex == -1)
        {
            throw new InvalidOperationException("Cache rebuild starting target not found.");
        }

        for (int i = startIndex; i < QuoteSupplier.Cache.Count; i++)
        {
            TQuote quote = QuoteSupplier.Cache[i];
            OnNext((Act.AddNew, quote));
        }
    }

    // delete cache entries, gracefully (and optionally, notifies observers)
    internal void ClearCache()
    {
        // nothing to do
        if (Cache.Count == 0)
        {
            Cache = [];
            Chain = [];
            return;
        }

        // reset from date
        DateTime fromDate = Cache[0].TickDate;
        ClearCache(fromDate);
    }

    // delete cache entries after fromDate (inclusive)
    internal void ClearCache(DateTime fromDate)
    {
        // index range
        int s = Cache.FindIndex(fromDate);
        int e = Cache.Count - 1;

        if (s == -1)
        {
            throw new InvalidOperationException("Cache clear starting target not found.");
        }

        ClearCache(s, e);
    }

    // delete cache entries between index range values (implemented in inheritor)
    internal abstract void ClearCache(int fromIndex, int toIndex);
}
