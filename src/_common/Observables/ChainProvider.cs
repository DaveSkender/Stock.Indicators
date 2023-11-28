namespace Skender.Stock.Indicators;

// STREAM INDICATOR CHAINOR

public abstract class ChainProvider : IObservable<(Act, DateTime, double)>
{
    // fields
    internal readonly List<IObserver<(Act, DateTime, double)>> observers;

    // constructor
    internal ChainProvider()
    {
        observers = [];
    }

    // METHODS

    public virtual void OnError(Exception error) => throw error;

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

    // notify observers
    internal void NotifyObservers((Act, DateTime, double) chainMessage)
        => observers.Notify(chainMessage);

    // notify observers (helper, for IReusableResult)
    internal void NotifyObservers(Act act, IReusableResult r)
    {
        (Act, DateTime, double) chainMessage = (act, r.Date, r.Value);
        observers.Notify(chainMessage);
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
