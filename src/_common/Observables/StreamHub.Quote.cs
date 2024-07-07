namespace Skender.Stock.Indicators;

// QUOTE HUB (STREAMING)

/// <summary>
/// QuoteHub distributes aggregegate OHLCV financial
/// market price information in generic IQuote interface type.
/// This Hub can also be an observer that redistribute quotes
/// from other <see cref="QuoteHub{TQuote}"/> hubs.
/// <para>
/// You can <see cref="Add(TQuote)"/> single or batch quotes.
/// It can be observed by external subscribers.
/// </para>
/// </summary>
/// <typeparam name="TQuote" cref="IQuote">
/// OHLCV price quote with value-based equality comparer
/// </typeparam>
public class QuoteHub<TQuote>
    : QuoteProvider<TQuote>, IQuoteHub<TQuote>
    where TQuote : struct, IQuote
{
    public QuoteHub() : this(cache: new()) { }

    public QuoteHub(
        QuoteProvider<TQuote> provider)
        : this(provider, cache: new()) { }

    private QuoteHub(
        StreamCache<TQuote> cache)
        : this(new QuoteProvider<TQuote>(cache), cache) { }

    private QuoteHub(
        QuoteProvider<TQuote> provider,
        StreamCache<TQuote> cache) : base(cache)
    {
        Supplier = provider;
        Observer = new(this, cache, this, provider);

        Reinitialize();
    }

    public StreamProvider<TQuote> Supplier { get; }

    public StreamObserver<TQuote, TQuote> Observer { get; }

    // hub

    public override string ToString()
        => $"QUOTES - {Results.Count} items (type: {nameof(TQuote)})";

    // cache

    public IReadOnlyList<TQuote> Results => StreamCache.Cache;

    public IReadOnlyList<TQuote> Quotes => StreamCache.Cache;

    // provider

    public Act Add(TQuote quote)
    {
        try
        {
            Act act = StreamCache.Modify(quote);
            NotifyObservers(act, quote);
            return act;
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }

    public void Add(IEnumerable<TQuote> quotes)
    {
        foreach (TQuote quote in quotes.ToSortedList())
        {
            Add(quote);
        }
    }

    public Act Delete(TQuote quote)
    {
        try
        {
            Act act = StreamCache.Purge(quote);
            NotifyObservers(act, quote);
            return act;
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }

    // observer

    public void OnNextNew(TQuote newItem)
    {
        // save and pass along
        Act act = StreamCache.Modify(newItem);
        NotifyObservers(act, newItem);
    }

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
