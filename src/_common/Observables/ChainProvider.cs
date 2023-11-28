namespace Skender.Stock.Indicators;

// STREAM INDICATOR CHAINOR

public abstract class ChainProvider<TResult> : IProvider<TResult>,
    IObservable<(Act, DateTime, double)>
    where TResult : IReusableResult, new()
{
    // fields
    private readonly List<IObserver<(Act, DateTime, double)>> observers;

    // constructor
    internal ChainProvider()
    {
        observers = [];
        Cache = [];
        LastArrival = new();
        OverflowCount = 0;
    }

    // PROPERTIES

    public IEnumerable<TResult> Results => Cache;

    public List<TResult> Cache { get; set; }

    public TResult LastArrival { get; set; }

    public int OverflowCount { get; set; }  // TODO: why can't these properties be internal set?

    // METHODS

    public void OnError(Exception error) => throw error;

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
        foreach (IObserver<(Act, DateTime, double)> obs in observers.ToArray())
        {
            if (observers.Contains(obs))
            {
                obs.OnCompleted();
            }
        }

        observers.Clear();
    }

    // delete cache, gracefully
    public void ResetCache()
    {
        int length = Cache.Count;

        if (length > 0)
        {
            // delete and deliver instruction,
            // in reverse order to prevent recompositions
            for (int i = length - 1; i > 0; i--)
            {
                TResult r = Cache[i];
                Act act = this.CacheWithAction(Act.Delete, r);
                NotifyObservers(act, r!);
            }
        }
    }

    // notify observers
    internal void NotifyObservers((Act, DateTime, double) chainMessage)
    {
        List<IObserver<(Act, DateTime, double)>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(Act, DateTime, double)> obs = obsList[i];
            obs.OnNext(chainMessage);
        }
    }

    // notify observers (helper, for IReusableResult)
    internal void NotifyObservers(Act act, IReusableResult r)
    {
        (Act, DateTime, double) t = (act, r.Date, r.Value);
        NotifyObservers(t);
    }

    // unsubscriber
    private class Unsubscriber(
        List<IObserver<(Act, DateTime, double)>> observers,
        IObserver<(Act, DateTime, double)> observer) : IDisposable
    {
        // can't mutate and iterate on same list, make copy
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
