namespace Skender.Stock.Indicators;

// STREAM PROVIDER (OBSERVABLE BASE)

/// <inheritdoc cref="IStreamProvider{TSeries}"/>
public abstract partial class StreamProvider<TSeries>
    : StreamCache<TSeries>, IStreamProvider<TSeries>
    where TSeries : ISeries
{
    private readonly HashSet<IObserver<(Act, TSeries, int?)>> _subscribers = [];

    public bool HasSubscribers => _subscribers.Count > 0;

    public int SubscriberCount => _subscribers.Count;

    public IReadOnlyList<TSeries> ReadCache => Cache;

    // SUBSCRIPTION SERVICES

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Act, TSeries, int?)> observer)
    {
        _subscribers.Add(observer);
        return new Subscription(_subscribers, observer);
    }

    // check if observer is subscribed
    public bool HasSubscriber(
        IObserver<(Act, TSeries, int?)> observer)
            => _subscribers.Contains(observer);

    /// <summary>
    /// A disposable subscription to the stream provider.
    /// <para>Unsubscribed with <see cref="Dispose()"/></para>
    /// </summary>
    /// <param name="subscribers">
    /// Registry of all subscribers (by ref)
    /// </param>
    /// <param name="subscriber">
    /// Your unique subscription as provided.
    /// </param>
    private class Subscription(
        ISet<IObserver<(Act, TSeries, int?)>> subscribers,
        IObserver<(Act, TSeries, int?)> subscriber) : IDisposable
    {
        // remove single observer
        public void Dispose() => subscribers.Remove(subscriber);
    }

    // unsubscribe all observers
    public void EndTransmission()
    {
        foreach (IObserver<(Act, TSeries, int?)> subscriber
            in _subscribers.ToArray())
        {
            if (_subscribers.Contains(subscriber))
            {
                subscriber.OnCompleted();
            }
        }

        _subscribers.Clear();
    }
}
