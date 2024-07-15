namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming provider (abstract subject).
/// <remarks>
/// Contains cache and provides observable stream.
/// </remarks>
/// </summary>
public abstract class StreamProvider<TSeries>
    : StreamCache<TSeries>, IStreamProvider<TSeries>
    where TSeries : ISeries
{
    private readonly HashSet<IObserver<(Act, TSeries)>> _observers = [];

    public bool HasSubscribers => _observers.Count > 0;

    public int SubscriberCount => _observers.Count;

    public IReadOnlyList<TSeries> ReadCache => Cache;

    #region METHODS (OBSERVABLE)

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Act, TSeries)> observer)
    {
        _observers.Add(observer);
        return new Subscription(_observers, observer);
    }

    // unsubscribe all observers
    public void EndTransmission()
    {
        foreach (IObserver<(Act, TSeries)> obs
            in _observers.ToArray())
        {
            if (_observers.Contains(obs))
            {
                obs.OnCompleted();
            }
        }

        _observers.Clear();
    }

    /// <summary>
    /// Sends <c>TSeries</c> item to all subscribers
    /// </summary>
    /// <param name="act" cref="Act">Caching instruction</param>
    /// <param name="item"><c>TSeries</c> item to send</param>
    protected void NotifyObservers(Act act, TSeries item)
    {
        // do not propogate "do nothing" acts
        if (act == Act.DoNothing)
        {
            return;
        }

        // send to subscribers
        foreach (IObserver<(Act, TSeries)> obs
            in _observers.ToArray())
        {
            obs.OnNext((act, item));
        }
    }

    /// <summary>
    /// A disposable subscription to the stream provider.
    /// <para>Unsubscribed with <see cref="Dispose()"/></para>
    /// </summary>
    /// <param name="observers">
    /// Registry of all subscribers (by ref)
    /// </param>
    /// <param name="observer">
    /// Your unique subscription as provided.
    /// </param>
    private class Subscription(
        ISet<IObserver<(Act, TSeries)>> observers,
        IObserver<(Act, TSeries)> observer) : IDisposable
    {
        // remove single observer
        public void Dispose() => observers.Remove(observer);
    }
    #endregion

    #region METHODS (CACHE CLEAR)

    /// clear cache without restore, from timestamp
    /// <inheritdoc/>
    public override void ClearCache(DateTime fromTimestamp)
    {
        // start of range
        int fromIndex = Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

        // something to do
        if (fromIndex != -1)
        {
            ClearCache(fromIndex);
        }
    }

    /// clear cache without restore, from index
    /// <inheritdoc/>
    public override void ClearCache(int fromIndex)
        => ClearCache(fromIndex, toIndex: Cache.Count - 1);

    /// <summary>
    /// Deletes cache entries between index range values.
    /// </summary>
    /// <remarks>
    /// This is implemented in inheriting (provider) class
    /// due to unique requirement to notify subscribers.
    /// </remarks>
    /// <param name="fromIndex">First element to delete</param>
    /// <param name="toIndex">Last element to delete</param>
    /// clears cache segment
    /// <inheritdoc />
    private void ClearCache(
        int fromIndex, int toIndex)
    {
        // nothing to do
        if (Cache.Count is 0)
        {
            return;
        }

        // determine in-range start/end indices
        int fr = Math.Max(0, Math.Min(fromIndex, toIndex));
        int to = Math.Min(Cache.Count - 1, Math.Max(fromIndex, toIndex));

        // delete and deliver instruction in reverse
        // order to prevent recursive recompositions
        for (int i = to; i >= fr; i--)
        {
            TSeries item = Cache[i];

            Act act = Modify(Act.Delete, item);
            NotifyObservers(act, item);
        }
    }
    #endregion
}
