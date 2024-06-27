namespace Skender.Stock.Indicators;

// STREAM PROVIDERS (BASE)

#region interface variants
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

/// <summary>
/// Streaming provider (abstract base)
/// </summary>
public abstract class AbstractProvider<TSeries>
    : AbstractCache<TSeries>, IStreamProvider<TSeries>
    where TSeries : struct, ISeries
{
    // reminder: these are the provider members only

    // fields
    private readonly HashSet<IObserver<(Act, TSeries)>> Observers = new();

    // PROPERTIES

    public bool HasSubscribers => Observers.Count > 0;

    public int SubscriberCount => Observers.Count;

    // METHODS

    // string label
    public abstract override string ToString();

    // subscribe observer
    public IDisposable Subscribe(IObserver<(Act, TSeries)> observer)
    {
        Observers.Add(observer);
        Subscription sub = new(Observers, observer);
        Resend(observer, act: Act.AddNew, fromIndex: 0); // catch-up new guy
        return sub;
    }

    // unsubscribe all observers
    public void EndTransmission()
    {
        foreach (IObserver<(Act, TSeries)> obs in Observers.ToArray())
        {
            if (Observers.Contains(obs))
            {
                obs.OnCompleted();
            }
        }

        Observers.Clear();
    }

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

    // resend to an observer
    /// <inheritdoc />
    public void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex,
        int? toIndex = null)
        => Resend(toObserver, fromIndex, default, toIndex);

    private void Resend(
        IObserver<(Act, TSeries)> toObserver,
        int fromIndex,
        Act act,
        int? toIndex = null)
    {
        if (toObserver is not null && Observers.Contains(toObserver))
        {
            // determine start/end of range
            int fr = Math.Max(0, fromIndex);
            int to = Math.Min(toIndex ?? Cache.Count - 1, Cache.Count - 1);

            for (int i = fr; i < to; i++)
            {
                toObserver.OnNext((act, Cache[i]));
            }
        }
    }

    // clears cache segment
    /// <inheritdoc />
    /// <remarks>
    /// Since upstream allows +/- offset, the arguments
    /// need to be evaluated to determine start/end of range.
    /// </remarks>
    protected override void ClearCache(
        int fromIndex, int? toIndex = null)
    {
        if (Cache.Count is 0)
        {
            return;
        }

        // determine start/end of range
        int fr = Math.Max(fromIndex, 0);
        int to = Math.Min(toIndex ?? Cache.Count - 1, Cache.Count - 1);

        // delete and deliver instruction in reverse
        // order to prevent recursive recompositions
        for (int i = to; i >= fr; i--)
        {
            TSeries r = Cache[i];
            Act act = ModifyCache(Act.Delete, r);
            NotifyObservers(act, r);
        }
    }

    /// <summary>
    /// Rebuild cache from provider.
    /// </summary>
    /// <param name="fromIndex">start position</param>
    /// <param name="toIndex">stop position</param>
    protected abstract void RebuildCache(
        int fromIndex, int? toIndex = null);

    /// <summary>
    /// Sends new item to all subscribers
    /// </summary>
    /// <param name="act"></param>
    /// <param name="item"></param>
    protected void NotifyObservers(Act act, TSeries item)
    {
        // do not propogate "do nothing" acts
        if (act == Act.DoNothing)
        {
            return;
        }

        // send to subscribers
        List<IObserver<(Act, TSeries)>> obsList = [.. Observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(Act, TSeries)> obs = obsList[i];
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
}
