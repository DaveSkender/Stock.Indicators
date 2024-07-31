namespace Skender.Stock.Indicators;

// STREAM PROVIDER (OBSERVABLE BASE)

/// <inheritdoc cref="IStreamObservable{TSeries}"/>
public abstract partial class StreamProvider<TSeries>
    : StreamCache<TSeries>, IStreamObservable<TSeries>
    where TSeries : ISeries
{
    private readonly HashSet<IStreamObserver<(Act, TSeries, int?)>> _subscribers = [];

    public bool HasSubscribers => _subscribers.Count > 0;

    public int SubscriberCount => _subscribers.Count;

    public IReadOnlyList<TSeries> ReadCache => Cache;

    // SUBSCRIPTION SERVICES

    // subscribe observer
    public IDisposable Subscribe(IStreamObserver<(Act, TSeries, int?)> observer)
    {
        _subscribers.Add(observer);
        return new Subscription(_subscribers, observer);
    }

    // check if observer is subscribed
    public bool HasSubscriber(
        IStreamObserver<(Act, TSeries, int?)> observer)
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
        ISet<IStreamObserver<(Act, TSeries, int?)>> subscribers,
        IStreamObserver<(Act, TSeries, int?)> subscriber) : IDisposable
    {
        // remove single observer
        public void Dispose() => subscribers.Remove(subscriber);
    }

    // unsubscribe all observers
    public void EndTransmission()
    {
        foreach (IStreamObserver<(Act, TSeries, int?)> subscriber
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
