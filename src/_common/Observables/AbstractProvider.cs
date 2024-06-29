namespace Skender.Stock.Indicators;

// STREAM PROVIDERS (BASE)
// with Quote, Chain, Result variants (at bottom)

/// <summary>
/// Streaming provider (abstract base)
/// </summary>
public abstract class AbstractProvider<TSeries>
    : AbstractCache<TSeries>, IStreamProvider<TSeries>
    where TSeries : struct, ISeries
{
    // provider members only

    private readonly HashSet<IObserver<(Act, TSeries)>> _observers = [];

    public bool HasSubscribers => _observers.Count > 0;

    public int SubscriberCount => _observers.Count;


    # region METHODS (OBSERVABLE)

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Act, TSeries)> observer)
    {
        _observers.Add(observer);
        Subscription sub = new(_observers, observer);
        Resend(observer, act: Act.AddNew, fromIndex: 0); // catch-up new guy
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
    protected void NotifyObservers(Act act, TSeries item)
    {
        // do not propogate "do nothing" acts
        if (act == Act.DoNothing)
        {
            return;
        }

        // send to subscribers
        List<IObserver<(Act, TSeries)>> obsList = [.. _observers];

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

    #region METHODS (UTILITIES)

    /// string label
    public abstract override string ToString();

    /// resend newer to an observer (from timestamp)
    /// <inhertitdoc />
    public void Resend(
        IObserver<(Act, TSeries)> toObserver,
        DateTime fromTimestamp)
    {
        int fromIndex = Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

        if (fromIndex == -1)
        {
            throw new InvalidOperationException(
                "Cache rebuild starting date not found.");
        }

        Resend(toObserver, fromIndex);
    }

    /// resend newer to an observer (from index)
    /// <inheritdoc />
    public void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex)
        => Resend(toObserver, fromIndex, Cache.Count - 1);

    /// resends values in a range to a requesting observer
    /// <inheritdoc />
    public void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex,
        int toIndex)
        => Resend(toObserver, Act.AddOld, fromIndex, toIndex);

    /// <summary>
    /// Resends values in a range to a requesting observer,
    /// between the provided indexes (inclusive), with a specific act.
    /// </summary>
    /// <param name="toObserver">Subscription identity</param>
    /// <param name="fromIndex">Starting index, inclusive</param>
    /// <param name="toIndex">Ending index, inclusive</param>
    /// <param name="act" cref="Act">Caching instruction</param>
    private void Resend(
        IObserver<(Act, TSeries)> toObserver,
        Act act,
        int fromIndex,
        int? toIndex = null)
    {
        if (toObserver is null || !_observers.Contains(toObserver))
        {
            return;
        }

        // determine start/end of range
        int fr = Math.Max(0, fromIndex);
        int to = Math.Min(toIndex ?? Cache.Count, Cache.Count);

        for (int i = fr; i < to; i++)
        {
            toObserver.OnNext((act, Cache[i]));
        }
    }

    /// clears cache segment
    /// <inheritdoc />
    protected override void ClearCache(
        int fromIndex, int toIndex)
    {
        if (Cache.Count is 0)
        {
            return;
        }

        // determine start/end of range
        int fr = Math.Max(fromIndex, 0);
        int to = Math.Min(toIndex, Cache.Count - 1);

        // delete and deliver instruction in reverse
        // order to prevent recursive recompositions
        for (int i = to; i >= fr; i--)
        {
            TSeries r = Cache[i];
            Act act = ModifyCache(Act.Delete, r);
            NotifyObservers(act, r);
        }
    }
    #endregion
}

#region QUOTE, CHAIN, RESULT PROVIDERS

/// <summary>
/// Quote provider (abstract base)
/// </summary>
public abstract class AbstractQuoteProvider<TQuote>
    : AbstractProvider<TQuote>, IQuoteProvider<TQuote>, IChainProvider<TQuote>
    where TQuote : struct, IQuote, IReusable
{
    // string label
    public override string ToString()
        => $"{Cache.Count} quotes (type: {nameof(TQuote)})";
}

/// <summary>
/// Chainable result provider (abstract base)
/// </summary>
public abstract class AbstractChainProvider<TReusableResult>
    : AbstractProvider<TReusableResult>, IChainProvider<TReusableResult>
    where TReusableResult : struct, IReusable;

/// <summary>
/// Non-chainable result provider (abstract base)
/// </summary>
public abstract class AbstractResultProvider<TResult>
    : AbstractProvider<TResult>, IResultProvider<TResult>
    where TResult : struct, IResult;
#endregion
