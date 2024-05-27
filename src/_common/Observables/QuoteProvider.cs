namespace Skender.Stock.Indicators;

// QUOTE PROVIDER

public class QuoteProvider<TQuote>
    : QuoteCache<TQuote>, IQuoteProvider
    where TQuote : IQuote, new()
{
    // fields
    private readonly List<IObserver<(Act, IQuote)>> observers;

    // constructor
    public QuoteProvider()
    {
        observers = [];
        Cache = [];

        Initialize();
    }

    // METHODS

    // string label
    public override string ToString()
        => $"Quote Provider ({Cache.Count} items)";

    /// <summary>
    /// Add a single quote.  We'll determine if it's new or an update.
    /// </summary>
    /// <param name="quote">Quote to add or update</param>
    public Act Add(TQuote quote)
    {
        try
        {
            Act act = CacheWithAnalysis(quote);

            NotifyObservers((act, quote));

            return act;
        }
        catch (OverflowException ox)
        {
            EndTransmission();

            string msg = "A repeated Quote update exceeded the 100 attempt threshold. "
                        + "Check and remove circular chains or check your Quote provider."
                        + "Provider terminated.";

            throw new OverflowException(msg, ox);
        }
    }

    // add many
    public void Add(IEnumerable<TQuote> quotes)
    {
        List<TQuote> added = quotes
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            Add(added[i]);
        }
    }

    /// <summary>
    /// Delete a quote.  We'll double-check that it exists in the
    /// cache before propogating the event to subscribers.
    /// </summary>
    /// <param name="quote">Quote to delete</param>
    public Act Delete(TQuote quote)
    {
        try
        {
            Act act = PurgeWithAnalysis(quote);
            NotifyObservers((act, quote));
            return act;
        }
        catch (OverflowException ox)
        {
            EndTransmission();

            string msg = "A repeated Quote delete exceeded the 100 attempt threshold. "
                        + "Check and remove circular chains or check your Quote provider."
                        + "Provider terminated.";

            throw new OverflowException(msg, ox);
        }

    }

    // re/initialize is graceful erase only for quote provider
    public void Initialize() => ClearCache();

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Act, IQuote)> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber(observers, observer);
    }

    // unsubscribe all observers
    public override void EndTransmission()
    {
        foreach (IObserver<(Act, IQuote)> obs in observers.ToArray())
        {
            if (observers.Contains(obs))
            {
                obs.OnCompleted();
            }
        }

        observers.Clear();
    }

    // delete cache, gracefully
    internal override void ClearCache(int fromIndex, int toIndex)
    {
        // delete and deliver instruction,
        // in reverse order to prevent recompositions
        for (int i = Cache.Count - 1; i > 0; i--)
        {
            TQuote q = Cache[i];
            Act act = CacheResultPerAction(Act.Delete, q);
            NotifyObservers((act, q));
        }

        // note: there is no auto-rebuild option since the
        // quote provider is a top level external entry point.
        // The using system will need to handle resupply with Add().
    }

    internal override void RebuildCache(DateTime fromDate, int offset)
        => throw new InvalidOperationException();

    internal override void RebuildCache(int fromIndex, int offset)
        => throw new InvalidOperationException();

    // notify observers
    private void NotifyObservers((Act act, IQuote quote) quoteMessage)
    {
        // do not propogate "do nothing" acts
        if (quoteMessage.act == Act.DoNothing)
        {
            return;
        }

        // send to subscribers
        List<IObserver<(Act, IQuote)>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(Act, IQuote)> obs = obsList[i];
            obs.OnNext(quoteMessage);
        }
    }

    // unsubscriber
    private class Unsubscriber(
        List<IObserver<(Act, IQuote)>> observers,
        IObserver<(Act, IQuote)> observer) : IDisposable
    {
        // can't mutate and iterate on same list, make copy
        private readonly List<IObserver<(Act, IQuote)>> observers = observers;
        private readonly IObserver<(Act, IQuote)> observer = observer;

        // remove single observer
        public void Dispose()
        {
            if (observer != null && observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }
    }
}
