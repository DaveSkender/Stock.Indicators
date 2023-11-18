namespace Skender.Stock.Indicators;

// QUOTE PROVIDER

public class QuoteProvider<TQuote>
    : IObservable<(Disposition, TQuote)>
    where TQuote : IQuote, new()
{
    // fields
    private readonly List<IObserver<(Disposition, TQuote)>> observers;

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
    public IDisposable Subscribe(IObserver<(Disposition, TQuote)> observer)
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
        foreach (IObserver<(Disposition, TQuote)> observer in observers.ToArray())
        {
            if (observers.Contains(observer))
            {
                observer.OnCompleted();
            }
        }

        observers.Clear();
    }

    // add one
    public Disposition Add(TQuote quote) => quote == null
        ? throw new ArgumentNullException(nameof(quote), "Quote cannot be null.")
        : CacheAndDeliverQuote(quote);

    // add many
    public void Add(IEnumerable<TQuote> quotes)
    {
        List<TQuote> added = quotes
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            Disposition disposition = CacheAndDeliverQuote(added[i]);
        }
    }

    // cache and deliver
    internal Disposition CacheAndDeliverQuote(TQuote quote)
    {
        // check for overflow and repeat arrival
        if (IsRepeat(quote))
        {
            // do not propogate
            Console.WriteLine("Do nothing with quote.");
            return Disposition.DoNothing;
        }

        // initialize
        Disposition disposition = new();
        int length = ProtectedQuotes.Count;

        // handle scenarios

        // first
        if (length == 0)
        {
            disposition = Disposition.AddNew;

            ProtectedQuotes.Add(quote);
            NotifyObservers((disposition, quote));

            return disposition;
        }

        TQuote last = ProtectedQuotes[length - 1];

        // newer
        if (quote.Date > last.Date)
        {
            disposition = Disposition.AddNew;
            ProtectedQuotes.Add(quote);
        }

        // current
        else if (quote.Date == last.Date)
        {
            disposition = Disposition.UpdateLast;
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
                disposition = Disposition.UpdateOld;
                ProtectedQuotes[i] = quote;
            }

            // add missing
            else
            {
                disposition = Disposition.AddOld;
                ProtectedQuotes.Add(quote);

                // re-sort cache
                ProtectedQuotes = ProtectedQuotes
                    .ToSortedList();
            }
        }

        // let observer handle
        NotifyObservers((disposition, quote));
        return disposition;
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
    private void NotifyObservers((Disposition, TQuote) value)
    {
        List<IObserver<(Disposition, TQuote)>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(Disposition, TQuote)> obs = obsList[i];
            obs.OnNext(value);
        }
    }

    // unsubscriber
    private class Unsubscriber(
        List<IObserver<(Disposition, TQuote)>> observers,
        IObserver<(Disposition, TQuote)> observer) : IDisposable
    {
        private readonly List<IObserver<(Disposition, TQuote)>> observers = observers;
        private readonly IObserver<(Disposition, TQuote)> observer = observer;

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
