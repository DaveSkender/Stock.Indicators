namespace Skender.Stock.Indicators;

// QUOTES as PROVIDER

public class QuoteProvider : IObservable<Quote>
{
    // fields
    private readonly List<IObserver<Quote>> observers;

    // initialize
    public QuoteProvider()
    {
        observers = new();
        ProtectedQuotes = new();
    }

    // properties
    public IEnumerable<Quote> Quotes => ProtectedQuotes;

    internal List<Quote> ProtectedQuotes { get; private set; }

    private int OverflowCount { get; set; }

    // METHODS

    // add one
    public void Add(Quote quote)
    {
        // validate quote
        if (quote == null)
        {
            throw new ArgumentNullException(nameof(quote), "Quote cannot be null.");
        }

        int length = ProtectedQuotes.Count;

        if (length == 0)
        {
            // add new quote
            ProtectedQuotes.Add(quote);

            // notify observers
            NotifyObservers(quote);

            return;
        }

        Quote last = ProtectedQuotes[length - 1];

        // add quote
        if (quote.Date > last.Date)
        {
            // add new quote
            ProtectedQuotes.Add(quote);

            // notify observers
            NotifyObservers(quote);
        }

        // same date or quote recieved
        else if (quote.Date <= last.Date)
        {
            // check for overflow condition
            // where same quote continues (possible circular condition)
            if (quote.Date == last.Date)
            {
                OverflowCount++;

                if (OverflowCount > 100)
                {
                    string msg = "A repeated Quote update exceeded the 100 attempt threshold. "
                      + "Check and remove circular chains or check your Quote provider.";

                    EndTransmission();

                    throw new OverflowException(msg);
                }
            }
            else
            {
                OverflowCount = 0;
            }

            // seek old quote
            int foundIndex = ProtectedQuotes
                .FindIndex(x => x.Date == quote.Date);

            // found
            if (foundIndex >= 0)
            {
                Quote old = ProtectedQuotes[foundIndex];

                old.Open = quote.Open;
                old.High = quote.High;
                old.Low = quote.Low;
                old.Close = quote.Close;
                old.Volume = quote.Volume;
            }

            // add missing quote
            else
            {
                ProtectedQuotes.Add(quote);

                // re-sort cache
                ProtectedQuotes = ProtectedQuotes
                    .ToSortedList();
            }

            // let observer handle old + duplicates
            NotifyObservers(quote);
        }
    }

    // add many
    public void Add(IEnumerable<Quote> quotes)
    {
        List<Quote> added = quotes
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            Add(added[i]);
        }
    }

    // subscribe
    public IDisposable Subscribe(IObserver<Quote> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber(observers, observer);
    }

    // close ALL observations
    public void EndTransmission()
    {
        foreach (IObserver<Quote> observer in observers.ToArray())
        {
            if (observers.Contains(observer))
            {
                observer.OnCompleted();
            }
        }

        observers.Clear();
    }

    // get history
    internal List<Quote> GetQuotesList() => ProtectedQuotes;

    // notify observers
    private void NotifyObservers(Quote quote)
    {
        List<IObserver<Quote>> obsList = observers.ToList();

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<Quote> obs = obsList[i];
            obs.OnNext(quote);
        }
    }

    // unsubscriber
    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<Quote>> observers;
        private readonly IObserver<Quote> observer;

        // identify and save observer
        public Unsubscriber(List<IObserver<Quote>> observers, IObserver<Quote> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }

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
