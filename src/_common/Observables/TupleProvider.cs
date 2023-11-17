namespace Skender.Stock.Indicators;

// TUPLE PROVIDER

public class TupleProvider : IObservable<(DateTime Date, double Value)>
{
    // fields
    private readonly List<IObserver<(DateTime Date, double Value)>> observers;

    internal TupleProvider()
    {
        observers = [];
        ProtectedTuples = [];
    }

    // PROPERTIES

    internal List<(DateTime Date, double Value)> ProtectedTuples { get; set; }

    internal int OverflowCount { get; set; }
    private (DateTime Date, double Value) LastArrival { get; set; }

    // METHODS

    // subscribe observer
    public IDisposable Subscribe(IObserver<(DateTime Date, double Value)> observer)
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
        foreach (IObserver<(DateTime Date, double Value)> observer in observers.ToArray())
        {
            if (observers.Contains(observer))
            {
                observer.OnCompleted();
            }
        }

        observers.Clear();
    }

    // add one
    internal void AddToTupleProvider((DateTime Date, double Value) tuple)
    {
        // check for overflow condition
        // where same tuple continues (possible circular condition)
        if (tuple == LastArrival)
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
            LastArrival = tuple;
        }

        // process arrival
        int length = ProtectedTuples.Count;

        // first
        if (length == 0)
        {
            ProtectedTuples.Add(tuple);
            NotifyObservers(tuple);
            return;
        }

        (DateTime Date, double Value) last = ProtectedTuples[length - 1];

        // newer
        if (tuple.Date > last.Date)
        {
            ProtectedTuples.Add(tuple);
        }

        // current
        else if (tuple.Date == last.Date)
        {
            last = tuple;
        }

        // late arrival
        // TODO: handle late arrivals
        else
        {
            throw new NotImplementedException();

            #pragma warning disable CS0162 // Unreachable code detected
            // seek duplicate
            int foundIndex = ProtectedTuples
                .FindIndex(x => x.Date == tuple.Date);

            // replace duplicate
            if (foundIndex >= 0)
            {
                ProtectedTuples[foundIndex] = tuple;
            }

            // add missing tuple
            else
            {
                ProtectedTuples.Add(tuple);

                // re-sort cache
                ProtectedTuples = ProtectedTuples
                    .ToSortedList();
            }
            #pragma warning restore CS0162 // Unreachable code detected
        }

        // let observer handle
        NotifyObservers(tuple);
    }

    // add one IReusableResult
    internal void AddToTupleProvider<TResult>(TResult result)
        where TResult : IReusableResult
    {
        (DateTime Date, double Value) tuple = result.ToTupleNaN();
        AddToTupleProvider(tuple);
    }

    // add many
    internal void AddToTupleProvider(IEnumerable<(DateTime Date, double Value)> tuples)
    {
        List<(DateTime Date, double Value)> added = tuples
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            AddToTupleProvider(added[i]);
        }
    }

    // notify observers
    internal void NotifyObservers((DateTime Date, double Value) tuple)
    {
        List<IObserver<(DateTime Date, double Value)>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(DateTime Date, double Value)> obs = obsList[i];
            obs.OnNext(tuple);
        }
    }

    // unsubscriber
    private class Unsubscriber(
        List<IObserver<(DateTime Date, double Value)>> observers,
        IObserver<(DateTime Date, double Value)> observer) : IDisposable
    {
        private readonly List<IObserver<(DateTime Date, double Value)>> observers = observers;
        private readonly IObserver<(DateTime Date, double Value)> observer = observer;

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
