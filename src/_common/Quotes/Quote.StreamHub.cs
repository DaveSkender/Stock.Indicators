namespace Skender.Stock.Indicators;

// QUOTE (STREAMING)

#region Hub interface
public interface IQuoteHub<TQuote> : IProviderHub
    where TQuote : struct, IQuote
{
    /// <summary>
    /// Add a single quote.
    /// We'll determine if it's new or an update.
    /// </summary>
    /// <param name="quote" cref="IQuote">
    /// Quote to add or update
    /// </param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    Act Add(TQuote quote);

    /// <summary>
    /// Add a batch of quotes.
    /// We'll determine if they're new or updated.
    /// </summary>
    /// <param name="quotes" cref="IQuote">
    ///   Batch of quotes to add or update
    /// </param>
    void Add(IEnumerable<TQuote> quotes);

    /// <summary>
    /// Delete a quote.  We'll double-check that it exists in the
    /// cache before propogating the event to subscribers.
    /// </summary>
    /// <param name="quote">Quote to delete</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    Act Delete(TQuote quote);
}
#endregion

/// <summary>
/// Quote provider, using generic IQuote interface type.
/// </summary>
/// <typeparam name="TQuote" cref="IQuote">
/// OHLCV price quote with value-based equality comparer
/// </typeparam>
public class QuoteHub<TQuote>
    : QuoteProvider<TQuote>, IQuoteHub<TQuote>
    where TQuote : struct, IQuote
{
    private readonly StreamCache<TQuote> _cache;

    public QuoteHub() : this(cache: new()) { }

    private QuoteHub(StreamCache<TQuote> cache)
        : base(cache)
    {
        _cache = cache;
    }


    // METHODS

    public override string ToString()
        => $"{_cache.Cache.Count} quotes (type: {nameof(TQuote)})";

    public Act Add(TQuote quote)
    {
        try
        {
            Act act = _cache.Modify(quote);
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
            Act act = _cache.Purge(quote);
            NotifyObservers(act, quote);
            return act;
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }
}
