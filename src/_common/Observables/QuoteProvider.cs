namespace Skender.Stock.Indicators;

// QUOTE PROVIDER

public class QuoteProvider<TQuote>
    : IObservable<(Act, TQuote)>
    where TQuote : IQuote, new()
{
    // fields
    private readonly List<IObserver<(Act, TQuote)>> observers;

    // constructor
    public QuoteProvider()
    {
        observers = [];
        ProtectedQuotes = [];
        LastArrival = new();
    }

    // PROPERTIES

    public IEnumerable<TQuote> Quotes => ProtectedQuotes;

    internal List<TQuote> ProtectedQuotes { get; private set; }

    private int OverflowCount { get; set; }
    private TQuote LastArrival { get; set; }

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

    // close all observations
    public void EndTransmission()
    {
        foreach (IObserver<(Act, TQuote)> observer in observers.ToArray())
        {
            if (observers.Contains(observer))
            {
                observer.OnCompleted();
            }
        }

        observers.Clear();
    }

    // add one
    public Act Add(TQuote quote) => quote == null
        ? throw new ArgumentNullException(nameof(quote), "Quote cannot be null.")
        : CacheAndDeliverQuote(quote);

    // add many
    public void Add(IEnumerable<TQuote> quotes)
    {
        List<TQuote> added = quotes
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            Act act = CacheAndDeliverQuote(added[i]);
        }
    }

    // cache and deliver
    internal Act CacheAndDeliverQuote(TQuote quote)
    {
        // check for overflow and repeat arrival
        if (IsRepeat(quote))
        {
            // do not propogate
            return Act.DoNothing;
        }

        // initialize
        Act act = new();
        int length = ProtectedQuotes.Count;

        // handle scenarios

        // first
        if (length == 0)
        {
            act = Act.AddNew;

            ProtectedQuotes.Add(quote);
            NotifyObservers((act, quote));

            return act;
        }

        TQuote last = ProtectedQuotes[length - 1];

        // newer
        if (quote.Date > last.Date)
        {
            act = Act.AddNew;
            ProtectedQuotes.Add(quote);
        }

        // current
        else if (quote.Date == last.Date)
        {
            act = Act.UpdateLast;
            last = quote;
        }

        // late arrival
        else
        {
            // find
            int i = ProtectedQuotes
                .FindIndex(x => x.Date == quote.Date);

            // replace
            if (i != -1)
            {
                act = Act.UpdateOld;
                ProtectedQuotes[i] = quote;
            }

            // add missing
            else
            {
                act = Act.AddOld;
                ProtectedQuotes.Add(quote);

                // re-sort cache
                ProtectedQuotes = ProtectedQuotes
                    .ToSortedList();
            }
        }

        // let observer handle
        NotifyObservers((act, quote));
        return act;
    }

    // evaluate overflow condition
    private bool IsRepeat(TQuote quote)
    {
        // check for overflow and repeat condition
        // where same tuple continues (possible circular condition)

        if (quote.IsEqual(LastArrival))
        {
            OverflowCount++;

            if (OverflowCount > 100)
            {
                EndTransmission();

                string msg = "A repeated Quote update exceeded the 100 attempt threshold. "
                           + "Check and remove circular chains or check your Quote provider."
                           + "Provider terminated.";

                throw new OverflowException(msg);
            }
            return true;
        }
        else
        {
            OverflowCount = 0;
            LastArrival = quote;
            return false;
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
