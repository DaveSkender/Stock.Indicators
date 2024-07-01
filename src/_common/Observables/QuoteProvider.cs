namespace Skender.Stock.Indicators;

/// <summary>
/// Quote provider, using generic IQuote interface type.
/// </summary>
/// <typeparam name="TQuote" cref="IQuote">
///   OHLCV price quote with value-based equality comparer
/// </typeparam>
public class QuoteProvider<TQuote> : AbstractQuoteProvider<TQuote>
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
    public Act Add(TQuote quote)
    {
        try
        {
            Act act = CacheWithAnalysis(quote);

            NotifyObservers(act, quote);

            return act;
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }

    /// <summary>
    /// Add a batch of quotes.
    /// We'll determine if they're new or updated.
    /// </summary>
    /// <param name="quotes" cref="IQuote">
    ///   Batch of quotes to add or update
    /// </param>
    public void Add(IEnumerable<TQuote> quotes)
    {
        foreach (TQuote quote in quotes.ToSortedList())
        {
            Add(quote);
        }
    }

    /// <summary>
    /// Delete a quote.  We'll double-check that it exists in the
    /// cache before propogating the event to subscribers.
    /// </summary>
    /// <param name="quote">Quote to delete</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    public Act Delete(TQuote quote)
    {
        try
        {
            Act act = PurgeWithAnalysis(quote);
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
