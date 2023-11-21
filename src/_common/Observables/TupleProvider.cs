namespace Skender.Stock.Indicators;

// TUPLE PROVIDER

public class TupleProvider
    : IObservable<(Act, DateTime, double)>
{
    // fields
    private readonly List<IObserver<(Act, DateTime, double)>> observers;

    // constructor
    internal TupleProvider()
    {
        observers = [];
        ProtectedTuples = [];
        LastArrival = new();
    }

    // PROPERTIES

    public IEnumerable<(DateTime Date, double Value)> Tuples => ProtectedTuples;

    internal List<(DateTime Date, double Value)> ProtectedTuples { get; set; }

    internal int OverflowCount { get; set; }
    internal (DateTime, double) LastArrival { get; set; }

    // METHODS

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Act, DateTime, double)> observer)
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
        foreach (IObserver<(Act, DateTime, double)> observer in observers.ToArray())
        {
            if (observers.Contains(observer))
            {
                observer.OnCompleted();
            }
        }

        observers.Clear();
    }

    // add one
    public Act Add((DateTime date, double value) price)
    {
        // initialize
        Act act = new();
        int length = ProtectedTuples.Count;

        // determine act

        // first
        if (length == 0)
        {
            act = Act.AddNew;
            return CacheAndDeliverTuple((act, price.date, price.value));
        }

        (DateTime date, double value) = ProtectedTuples[length - 1];

        // newer
        if (price.date > date)
        {
            act = Act.AddNew;
        }

        // current
        else if (price.date == date)
        {
            act = Act.UpdateLast;
        }

        // late arrival
        else
        {
            // seek duplicate
            int foundIndex = ProtectedTuples
                .FindIndex(x => x.Date == price.date);

            // replace duplicate
            act = foundIndex == -1 ? Act.AddOld : Act.UpdateOld;
        }

        return CacheAndDeliverTuple(
            (act, price.date, price.value));
    }

    // add one IReusableResult
    internal Act HandleInboundResult<TResult>(
        Act act, TResult result)
        where TResult : IReusableResult
        => CacheAndDeliverTuple(
            (act, result.Date, result.Value));

    // cache and deliver
    internal Act CacheAndDeliverTuple(
        (Act act, DateTime date, double value) t)
    {
        // check for overflow, repeat arrival, do nothing instruction
        if (t.act == Act.DoNothing
            || IsRepeat((t.date, t.value)))
        {
            // do not propogate
            return Act.DoNothing;
        }

        // initialize
        (DateTime date, double value) tuple = (t.date, t.value);
        int length = ProtectedTuples.Count;

        // save tuple
        switch (t.act)
        {
            case Act.AddNew:

                ProtectedTuples.Add(tuple);
                break;

            case Act.AddOld:

                ProtectedTuples.Add(tuple);

                // re-sort cache
                ProtectedTuples = ProtectedTuples
                    .ToSortedList();

                break;

            case Act.UpdateLast:

                (DateTime date, double value) = ProtectedTuples[length - 1];

                // update confirmed last entry
                if (date == tuple.date)
                {
                    value = tuple.value;
                }

                // failover to UpdateOld
                else
                {
                    t.act = Act.UpdateOld;
                    CacheAndDeliverTuple(t);
                }

                break;

            case Act.UpdateOld:

                // find
                int i = ProtectedTuples
                    .FindIndex(x => x.Date == tuple.date);

                // replace
                if (i != -1)
                {
                    ProtectedTuples[i] = tuple;
                }

                // failover to AddOld
                else
                {
                    t.act = Act.AddOld;
                    CacheAndDeliverTuple(t);
                }

                break;

            case Act.Delete:

                // find conservatively
                int d = ProtectedTuples
                    .FindIndex(x =>
                       x.Date == tuple.date
                    && x.Value == tuple.value);

                // delete
                if (d != -1)
                {
                    ProtectedTuples.RemoveAt(d);
                }

                // nothing to delete
                else
                {
                    t.act = Act.DoNothing;
                }

                break;

            case Act.DoNothing:
                // handled earlier
                break;

            default:
                throw new InvalidOperationException();
        }

        // let observer handle
        NotifyObservers(t);
        return t.act; ;
    }

    // evaluate overflow condition
    internal bool IsRepeat((DateTime date, double value) tuple)
    {
        // check for overflow and repeat condition
        // where same tuple continues (possible circular condition)

        if (tuple.IsEqual(LastArrival))
        {
            OverflowCount++;

            if (OverflowCount > 100)
            {
                EndTransmission();

                string msg = "A repeated Tuple update exceeded the 100 attempt threshold. "
                           + "Check and remove circular chains or check your Tuple provider."
                           + "Provider terminated.";

                throw new OverflowException(msg);
            }

            return true;
        }
        else
        {
            OverflowCount = 0;
            LastArrival = tuple;
            return false;
        }
    }

    // delete cache, gracefully
    internal void ResetTupleCache()
    {
        int length = ProtectedTuples.Count;

        if (length > 0)
        {
            // delete and deliver instruction,
            // in reverse order to prevent recompositions
            for (int i = length - 1; i > 0; i--)
            {
                (DateTime date, double value) = ProtectedTuples[i];
                CacheAndDeliverTuple((Act.Delete, date, value));
            }
        }
    }

    // notify observers
    private void NotifyObservers((Act, DateTime, double) value)
    {
        List<IObserver<(Act, DateTime, double)>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(Act, DateTime, double)> obs = obsList[i];
            obs.OnNext(value);
        }
    }

    // unsubscriber
    private class Unsubscriber(
        List<IObserver<(Act, DateTime, double)>> observers,
        IObserver<(Act, DateTime, double)> observer) : IDisposable
    {
        private readonly List<IObserver<(Act, DateTime, double)>> observers = observers;
        private readonly IObserver<(Act, DateTime, double)> observer = observer;

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
