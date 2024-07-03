namespace Skender.Stock.Indicators;

// STREAM PROVIDERS (BASE)
// with Quote, Chain, Result variants (at bottom)

#region QUOTE, CHAIN, RESULT PROVIDERS

/// <summary>
/// Quote provider (abstract base)
/// </summary>
public abstract class QuoteProvider<TQuote>
    : ChainProvider<TQuote>
    where TQuote : struct, IQuote
{
    protected internal QuoteProvider(
        StreamCache<TQuote> observableCache)
        : base(observableCache) { }
}

/// <summary>
/// Chainable result provider (abstract base)
/// </summary>
public abstract class ChainProvider<TReusableResult>
    : ResultProvider<TReusableResult>
    where TReusableResult : struct, IReusable
{
    protected internal ChainProvider(
        StreamCache<TReusableResult> observableCache)
        : base(observableCache) { }
}

/// <summary>
/// Non-chainable result provider (abstract base)
/// </summary>
public abstract class ResultProvider<TResult>
    : StreamProvider<TResult>
    where TResult : struct, IResult
{
    protected internal ResultProvider(
        StreamCache<TResult> observableCache)
        : base(observableCache) { }
}
#endregion

/// <summary>
/// Streaming provider (generic)
/// </summary>
public abstract class StreamProvider<TSeries> : IStreamProvider<TSeries>
    where TSeries : struct, ISeries
{
    // provider members only

    private readonly HashSet<IObserver<(Act, TSeries)>> _observers = [];
    private readonly StreamCache<TSeries> _cache;

    protected internal StreamProvider(
        StreamCache<TSeries> observableCache)
    {
        _cache = observableCache;
    }

    public bool IsFaulted => _cache.IsFaulted;

    public bool HasSubscribers => _observers.Count > 0;

    public int SubscriberCount => _observers.Count;

    public IReadOnlyList<TSeries> Results => _cache.Cache;

    internal List<TSeries> Cache => _cache.Cache;


    #region METHODS (OBSERVABLE)

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Act, TSeries)> observer)
    {
        _observers.Add(observer);
        Subscription sub = new(_observers, observer);
        Resend(observer, fromIndex: 0, act: Act.AddNew); // catch-up new guy
        return sub;
    }

    // unsubscribe all observers
    public void EndTransmission()
    {
        foreach (IObserver<(Act, TSeries)> obs in _observers.ToArray())
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
    internal void NotifyObservers(Act act, TSeries item)
    {
        // do not propogate "do nothing" acts
        if (act == Act.DoNothing)
        {
            return;
        }

        // send to subscribers
        IReadOnlyList<IObserver<(Act, TSeries)>> obsList
            = _observers.ToList();

        foreach (IObserver<(Act, TSeries)> obs in obsList)
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

    #region METHODS (OBSERVABLE UTILITIES)

    /// resend newer to an observer (from timestamp)
    /// <inhertitdoc />
    public void Resend(
        IObserver<(Act, TSeries)> toObserver,
        DateTime fromTimestamp,
        Act act)
    {
        int fromIndex = _cache.Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

        if (fromIndex == -1)
        {
            throw new InvalidOperationException(
                "Cache rebuild starting date not found.");
        }

        Resend(toObserver, fromIndex, act);
    }

    /// resend newer to an observer (from index)
    /// <inheritdoc />
    public void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex,
        Act act)
        => Resend(toObserver, fromIndex, _cache.Cache.Count - 1, act);

    /// resends values in a range to a requesting observer
    /// <inheritdoc />
    public void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex,
        int toIndex,
        Act act)
    {
        if (toObserver is null || !_observers.Contains(toObserver))
        {
            return;
        }

        // determine start/end of range
        int fr = Math.Max(0, fromIndex);
        int to = Math.Min(toIndex, _cache.Cache.Count - 1);

        for (int i = fr; i <= to; i++)
        {
            toObserver.OnNext((act, _cache.Cache[i]));
        }
    }
    #endregion

    #region METHODS (CACHE UTILITIES)

    /// clear cache without restore
    /// <inheritdoc/>
    public void ClearCache() => ClearCache(0);

    /// clear cache without restore, from timestamp
    /// <inheritdoc/>
    public void ClearCache(DateTime fromTimestamp)
    {
        // start of range
        int fromIndex = _cache.Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

        // something to do
        if (fromIndex != -1)
        {
            ClearCache(fromIndex);
        }
    }

    /// clear cache without restore, from index
    /// <inheritdoc/>
    public void ClearCache(int fromIndex)
        => ClearCache(fromIndex, toIndex: _cache.Cache.Count - 1);

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
    protected void ClearCache(
        int fromIndex, int toIndex)
    {
        // nothing to do
        if (_cache.Cache.Count is 0)
        {
            return;
        }

        // determine in-range start/end indices
        int fr = Math.Max(0, Math.Min(fromIndex, toIndex));
        int to = Math.Min(_cache.Cache.Count - 1, Math.Max(fromIndex, toIndex));

        // delete and deliver instruction in reverse
        // order to prevent recursive recompositions
        for (int i = to; i >= fr; i--)
        {
            TSeries r = _cache.Cache[i];
            Act act = _cache.ModifyCache(Act.Delete, r);
            NotifyObservers(act, r);
        }
    }

    #endregion
}
