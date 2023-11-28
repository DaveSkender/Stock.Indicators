namespace Skender.Stock.Indicators;

// QUOTE PROVIDER

public sealed class QuoteProvider<TQuote> : IProvider<TQuote>,
    IObservable<(Act, TQuote)>
    where TQuote : IQuote, new()
{
    // fields
    private readonly List<IObserver<(Act, TQuote)>> observers;

    // constructor
    public QuoteProvider()
    {
        observers = [];
        Cache = [];
        LastArrival = new();
        OverflowCount = 0;
    }

    // PROPERTIES

    public IEnumerable<TQuote> Quotes => Cache;

    public IEnumerable<TQuote> Results => Cache;

    public List<TQuote> Cache { get; set; }

    public TQuote LastArrival { get; set; }

    public int OverflowCount { get; set; }

    // METHODS

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Act, TQuote)> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber(observers, observer);
    }

    // unsubscribe all observers
    public void EndTransmission()
    {
        foreach (IObserver<(Act, TQuote)> obs in observers.ToArray())
        {
            if (observers.Contains(obs))
            {
                obs.OnCompleted();
            }
        }

        observers.Clear();
    }

    // add one
    public Act Add(TQuote quote)
    {
        Act act = this.CacheWithAnalysis(quote);

        NotifyObservers((act, quote));
        return act;
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

    // delete cache, gracefully
    public void ResetCache()
    {
        int length = Cache.Count;

        if (length > 0)
        {
            // delete and deliver instruction,
            // in reverse order to prevent recompositions
            for (int i = length - 1; i > 0; i--)
            {
                TQuote q = Cache[i];
                Act act = this.CacheWithAction(Act.Delete, q);
                NotifyObservers((act, q));
            }
        }
    }

    // notify observers
    private void NotifyObservers((Act, TQuote) value)
    {
        List<IObserver<(Act, TQuote)>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(Act, TQuote)> obs = obsList[i];
            obs.OnNext(value);
        }
    }

    // unsubscriber
    private class Unsubscriber(
        List<IObserver<(Act, TQuote)>> observers,
        IObserver<(Act, TQuote)> observer) : IDisposable
    {
        // can't mutate and iterate on same list, make copy
        private readonly List<IObserver<(Act, TQuote)>> observers = observers;
        private readonly IObserver<(Act, TQuote)> observer = observer;

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
