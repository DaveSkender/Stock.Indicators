namespace Skender.Stock.Indicators;

// TUPLE OBSERVER and TUPLE PROVIDER (CHAIN STREAM)

public abstract class ChainProvider
    : TupleObserver, IObservable<(DateTime Date, double Value)>
{
    // fields
    private readonly List<IObserver<(DateTime Date, double Value)>> observers;

    // constructor
    protected ChainProvider()
    {
        observers = new();
        ProtectedChain = new();
        IsWarmup = true;
    }

    // PROPERTIES

    internal IEnumerable<(DateTime Date, double Value)> Output => ProtectedChain;

    internal List<(DateTime Date, double Value)> ProtectedChain { get; set; }

    private int OverflowCount { get; set; }

    private bool IsWarmup { get; set; }

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

    // close all observations
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
    internal void SendToChain<TResult>(TResult result)
        where TResult : IReusableResult
    {
        // candidate result
        (DateTime Date, double Value) r = new(result.Date, result.Value.Null2NaN());

        int i = ProtectedChain.Count;

        // initialize
        if (i == 0 && result.Value != null)
        {
            // add new tuple
            ProtectedChain.Add(r);
            IsWarmup = false;

            // notify observers
            NotifyObservers(r);
            return;
        }

        // do not proceed until first non-null Value recieved
        if (IsWarmup && result.Value == null)
        {
            return;
        }
        else
        {
            IsWarmup = false;
        }

        (DateTime lastDate, _) = ProtectedChain[i - 1];

        // add tuple
        if (r.Date > lastDate)
        {
            // add new tuple
            ProtectedChain.Add(r);

            // notify observers
            NotifyObservers(r);
        }

        // same date or tuple recieved
        else if (r.Date <= lastDate)
        {
            // check for overflow condition
            // where same tuple continues (possible circular condition)
            if (r.Date == lastDate)
            {
                OverflowCount++;

                if (OverflowCount > 100)
                {
                    string msg = "A repeated Chain update exceeded the 100 attempt threshold. "
                      + "Check and remove circular chains or check your Chain provider.";

                    EndTransmission();

                    throw new OverflowException(msg);
                }
            }
            else
            {
                OverflowCount = 0;
            }

            // seek old tuple
            int foundIndex = ProtectedChain
                .FindIndex(x => x.Date == r.Date);

            // found
            if (foundIndex >= 0)
            {
                ProtectedChain[foundIndex] = r;
            }

            // add missing tuple
            else
            {
                ProtectedChain.Add(r);

                // re-sort cache
                ProtectedChain = ProtectedChain
                    .ToSortedList();
            }

            // let observer handle old + duplicates
            NotifyObservers(r);
        }
    }

    // add many
    internal void SendToChain<TResult>(IEnumerable<TResult> results)
        where TResult : IReusableResult
    {
        List<TResult> added = results
            .ToSortedList();

        for (int i = 0; i < added.Count; i++)
        {
            SendToChain(added[i]);
        }
    }

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
