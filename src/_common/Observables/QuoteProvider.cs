namespace Skender.Stock.Indicators;

// QUOTE PROVIDER
// TODO: update to TQuote
public class QuoteProvider : IObservable<Quote>
{
    // fields
    private readonly List<IObserver<Quote>> observers;

    // constructor
    public QuoteProvider()
    {
        observers = [];
        ProtectedQuotes = [];
        LastArrival = new();
    }

    // PROPERTIES

    public IEnumerable<Quote> Quotes => ProtectedQuotes;

    internal List<Quote> ProtectedQuotes { get; private set; }

    private int OverflowCount { get; set; }
    private Quote LastArrival { get; set; }

    // METHODS

    // subscribe observer
    public IDisposable Subscribe(IObserver<Quote> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber(observers, observer);
    }

    // close all observations
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

    // add one
    public void AddToQuoteProvider(Quote quote)
    {
        // validate quote
        if (quote == null)
        {
            throw new ArgumentNullException(nameof(quote), "Quote cannot be null.");
        }

        // check for overflow condition
        // where same tuple continues (possible circular condition)
        if (quote == LastArrival)
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
            LastArrival = quote;
        }

        // process arrival
        int length = ProtectedQuotes.Count;

        // first
        if (length == 0)
        {
            ProtectedQuotes.Add(quote);
            NotifyObservers(quote);
            return;
        }

        Quote last = ProtectedQuotes[length - 1];

        // newer
        if (quote.Date > last.Date)
        {
            ProtectedQuotes.Add(quote);
        }

        // current
        else if (quote.Date == last.Date)
        {
            last = quote;
        }

        // late arrival
        // TODO: handle late arrivals
        else
        {
            throw new NotImplementedException();

            #pragma warning disable CS0162 // Unreachable code detected
            // seek duplicate
            int foundIndex = ProtectedQuotes
                .FindIndex(x => x.Date == quote.Date);

            // replace duplicate
            if (foundIndex >= 0)
            {
                ProtectedQuotes[foundIndex] = quote;
            }

            // add missing quote
            else
            {
                ProtectedQuotes.Add(quote);

                // re-sort cache
                ProtectedQuotes = ProtectedQuotes
                    .ToSortedList();
            }
            #pragma warning restore CS0162 // Unreachable code detected
        }

        // let observer handle
        NotifyObservers(quote);
    }

    // add many
    public void AddToQuoteProvider(IEnumerable<Quote> quotes)
    {
        List<Quote> added = quotes
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            AddToQuoteProvider(added[i]);
        }
    }

    // notify observers
    private void NotifyObservers(Quote quote)
    {
        List<IObserver<Quote>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<Quote> obs = obsList[i];
            obs.OnNext(quote);
        }
    }

    // unsubscriber
    private class Unsubscriber(
        List<IObserver<Quote>> observers,
        IObserver<Quote> observer) : IDisposable
    {
        private readonly List<IObserver<Quote>> observers = observers;
        private readonly IObserver<Quote> observer = observer;

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
