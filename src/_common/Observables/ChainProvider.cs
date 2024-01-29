namespace Skender.Stock.Indicators;

// CHAIN PROVIDER

public abstract class ChainProvider : IChainProvider
{
    // fields
    private readonly List<IObserver<(Act, DateTime, double)>> observers;

    // constructor
    private protected ChainProvider()
    {
        observers = [];
        Chain = [];
    }

    // PROPERTIES

    internal List<(DateTime TickDate, double Value)> Chain;

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
    public virtual void EndTransmission()
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
        => NotifyObservers((act, r.TickDate, r.Value));

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
