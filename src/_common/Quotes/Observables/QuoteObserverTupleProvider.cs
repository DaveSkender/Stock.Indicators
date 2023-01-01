namespace Skender.Stock.Indicators;

// QUOTE OBSERVER and TUPLE PROVIDER

public class QuoteObserverTupleProvider
    : QuoteObserver, IObservable<(DateTime Date, double Value)>
{
    // fields
    private readonly List<IObserver<(DateTime Date, double Value)>> observers;

    // initialize
    public QuoteObserverTupleProvider()
    {
        observers = new();
        ProtectedOutput = new();
    }

    // properties
    public IEnumerable<(DateTime Date, double Value)> Output => ProtectedOutput;

    internal List<(DateTime Date, double Value)> ProtectedOutput { get; set; }

    private int OverflowCount { get; set; }

    // METHODS

    // add one
    public void Add((DateTime Date, double Value) tuple)
    {
        int length = ProtectedOutput.Count;

        if (length == 0)
        {
            // add new tuple
            ProtectedOutput.Add(tuple);

            // notify observers
            NotifyObservers(tuple);
            return;
        }

        (DateTime lastDate, _) = ProtectedOutput[length - 1];

        // add tuple
        if (tuple.Date > lastDate)
        {
            // add new tuple
            ProtectedOutput.Add(tuple);

            // notify observers
            NotifyObservers(tuple);
        }

        // same date or tuple recieved
        else if (tuple.Date <= lastDate)
        {
            // check for overflow condition
            // where same tuple continues (possible circular condition)
            if (tuple.Date == lastDate)
            {
                OverflowCount++;

                if (OverflowCount > 100)
                {
                    string msg = "A repeated Tuple update exceeded the 100 attempt threshold. "
                      + "Check and remove circular chains or check your Tuple provider.";

                    EndTransmission();

                    throw new OverflowException(msg);
                }
            }
            else
            {
                OverflowCount = 0;
            }

            // seek old tuple
            int foundIndex = ProtectedOutput
                .FindIndex(x => x.Date == tuple.Date);

            // found
            if (foundIndex >= 0)
            {
                ProtectedOutput[foundIndex] = tuple;
            }

            // add missing tuple
            else
            {
                ProtectedOutput.Add(tuple);

                // re-sort cache
                ProtectedOutput = ProtectedOutput
                    .ToSortedList();
            }

            // let observer handle old + duplicates
            NotifyObservers(tuple);
        }
    }

    // add many
    public void Add(IEnumerable<(DateTime Date, double Value)> tuples)
    {
        List<(DateTime Date, double Value)> added = tuples
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            Add(added[i]);
        }
    }

    // subscribe
    public IDisposable Subscribe(IObserver<(DateTime Date, double Value)> observer)
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
        foreach (IObserver<(DateTime Date, double Value)> observer in observers.ToArray())
        {
            if (observers.Contains(observer))
            {
                observer.OnCompleted();
            }
        }

        observers.Clear();
    }

    // get history
    internal List<(DateTime Date, double Value)> GetTuplesList() => ProtectedOutput;

    // notify observers
    private void NotifyObservers((DateTime Date, double Value) tuple)
    {
        List<IObserver<(DateTime Date, double Value)>> obsList = observers.ToList();

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(DateTime Date, double Value)> obs = obsList[i];
            obs.OnNext(tuple);
        }
    }

    // unsubscriber
    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<(DateTime Date, double Value)>> observers;
        private readonly IObserver<(DateTime Date, double Value)> observer;

        // identify and save observer
        public Unsubscriber(List<IObserver<(DateTime Date, double Value)>> observers, IObserver<(DateTime Date, double Value)> observer)
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
