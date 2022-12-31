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

    private Quote? LastQuote { get; set; }

    // METHODS

    // get history
    internal List<Quote> GetQuotesList() => ProtectedQuotes;

    // add many
    public void Add(Quote? quote)
    {
        // validate quote
        if (quote == null)
        {
            throw new ArgumentNullException(nameof(quote), "Quote cannot be null.");
        }

        // add quote
        if (LastQuote == null || quote.Date > LastQuote.Date)
        {
            // update last ref
            LastQuote = quote;

            // add new quote
            ProtectedQuotes.Add(quote);

            // notify
            foreach (IObserver<Quote> observer in observers)
            {
                observer.OnNext(quote);
            }
        }

        // same date or quote recieved
        else if (quote.Date <= LastQuote.Date)
        {
            // todo: how to reset stream, is that even possible?

            Quote? old = ProtectedQuotes
                .Find(quote.Date);

            // found
            if (old != null)
            {
                old.Open = quote.Open;
                old.High = quote.High;
                old.Low = quote.Low;
                old.Close = quote.Close;
                old.Volume = quote.Volume;
            }

            // add old missing record
            else
            {
                ProtectedQuotes.Add(quote);

                // resort cache
                ProtectedQuotes = ProtectedQuotes
                    .ToSortedList();
            }

            // let observer handle old + duplicates
            foreach (IObserver<Quote> observer in observers)
            {
                observer.OnNext(quote);
            }
        }
    }

    // add one
    public void Add(IEnumerable<Quote> quotes)
    {
        List<Quote> addedQuotes = quotes
            .ToSortedList();

        for (int i = 0; i < addedQuotes.Count; i++)
        {
            Add(addedQuotes[i]);
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
