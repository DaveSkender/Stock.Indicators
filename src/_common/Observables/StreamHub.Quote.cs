namespace Skender.Stock.Indicators;

// QUOTE HUB (STREAMING)

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
    public QuoteHub() : base(observableCache: new()) { }

    public IReadOnlyList<TQuote> Quotes => StreamCache.Cache;

    // METHODS

    public override string ToString()
        => $"{StreamCache.Cache.Count} quotes (type: {nameof(TQuote)})";

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
}
