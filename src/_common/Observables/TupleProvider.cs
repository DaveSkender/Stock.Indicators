namespace Skender.Stock.Indicators;

// TUPLE PROVIDER

public class TupleProvider
    : IObservable<(Disposition, DateTime, double)>
{
    // fields
    private readonly List<IObserver<(Disposition, DateTime, double)>> observers;

    // constructor
    internal TupleProvider()
    {
        observers = [];
        ProtectedTuples = [];
        LastArrival = new();
    }

    // PROPERTIES

    public IEnumerable<(DateTime Date, double Value)> ResultTuples => ProtectedTuples;

    internal List<(DateTime Date, double Value)> ProtectedTuples { get; set; }

    internal int OverflowCount { get; set; }
    private (DateTime Date, double Value) LastArrival { get; set; }

    // METHODS

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Disposition, DateTime, double)> observer)
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
        foreach (IObserver<(Disposition, DateTime, double)> observer in observers.ToArray())
        {
            if (observers.Contains(observer))
            {
                observer.OnCompleted();
            }
        }

        observers.Clear();
    }

    // add one
    public Disposition Add((DateTime date, double value) price)
    {
        // initialize 
        Disposition disposition = new();
        int length = ProtectedTuples.Count;

        // determine disposition

        // first
        if (length == 0)
        {
            return Disposition.AddNew;
        }

        (DateTime date, double value) = ProtectedTuples[length - 1];

        // newer
        if (price.date > date)
        {
            disposition = Disposition.AddNew;
        }

        // current
        else if (price.date == date)
        {
            disposition = Disposition.UpdateLast;
        }

        // late arrival
        else
        {
            // seek duplicate
            int foundIndex = ProtectedTuples
                .FindIndex(x => x.Date == price.date);

            // replace duplicate
            disposition = foundIndex >= 0 ? Disposition.UpdateOld : Disposition.AddOld;
        }

        return CacheAndDeliverTuple(
            (disposition, price.date, price.value));
    }

    // add one IReusableResult
    internal Disposition HandleInboundResult<TResult>(
        Disposition disposition, TResult result)
        where TResult : IReusableResult
        => CacheAndDeliverTuple(
            (disposition, result.Date, result.Value.Null2NaN()));

    // cache and deliver
    internal Disposition CacheAndDeliverTuple(
        (Disposition disposition, DateTime date, double value) t)
    {
        // check for overflow, repeat arrival, do nothing instruction
        if (t.disposition == Disposition.DoNothing
            || IsRepeat((t.date, t.value)))
        {
            // do not propogate
            return Disposition.DoNothing;
        }

        // initialize
        (DateTime date, double value) tuple = (t.date, t.value);
        int length = ProtectedTuples.Count;

        // handle instruction
        switch (t.disposition)
        {
            case Disposition.AddNew:

                ProtectedTuples.Add(tuple);
                break;

            case Disposition.AddOld:

                ProtectedTuples.Add(tuple);

                // re-sort cache
                ProtectedTuples = ProtectedTuples
                    .ToSortedList();

                break;

            case Disposition.UpdateLast:

                (DateTime date, double value) = ProtectedTuples[length - 1];

                // update confirmed last entry
                if (date == tuple.date)
                {
                    value = tuple.value;
                }

                // failover to UpdateOld
                else
                {
                    t.disposition = Disposition.UpdateOld;
                    CacheAndDeliverTuple(t);
                }

                break;

            case Disposition.UpdateOld:

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
                    t.disposition = Disposition.AddOld;
                    CacheAndDeliverTuple(t);
                }

                break;

            case Disposition.Delete:

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
                    t.disposition = Disposition.DoNothing;
                }

                break;

            case Disposition.DoNothing:
                // handled earlier
                break;

            default:
                throw new InvalidOperationException();
        }

        // let observer handle
        NotifyObservers(t);
        return t.disposition; ;
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
                CacheAndDeliverTuple((Disposition.Delete, date, value));
            }
        }
    }

    // notify observers
    private void NotifyObservers((Disposition, DateTime, double) value)
    {
        List<IObserver<(Disposition, DateTime, double)>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(Disposition, DateTime, double)> obs = obsList[i];
            obs.OnNext(value);
        }
    }

    // unsubscriber
    private class Unsubscriber(
        List<IObserver<(Disposition, DateTime, double)>> observers,
        IObserver<(Disposition, DateTime, double)> observer) : IDisposable
    {
        private readonly List<IObserver<(Disposition, DateTime, double)>> observers = observers;
        private readonly IObserver<(Disposition, DateTime, double)> observer = observer;

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
