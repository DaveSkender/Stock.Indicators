namespace Skender.Stock.Indicators;

// STREAM PROVIDERS (BASE)

#region interface variants
/// <summary>
/// Quote provider (abstract base)
/// </summary>
public abstract class AbstractQuoteProvider<TQuote>
    : AbstractProvider<TQuote>, IQuoteProvider<TQuote>, IChainProvider<TQuote>
    where TQuote : struct, IQuote, IReusableResult
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
    where TReusableResult : struct, IReusableResult;

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
        return new Subscription(Observers, observer);
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

    // resend to an observer
    /// <inheritdoc />
    public void Resend(
        int fromIndex,
        IObserver<(Act, TSeries)> toObserver)
    {
        if (toObserver is not null && Observers.Contains(toObserver))
        {
            for (int i = fromIndex; i < Cache.Count; i++)
            {
                toObserver.OnNext((Act.AddOld, Cache[i]));
            }
        }

        throw new NotImplementedException("unsure if needed");
    }

    // clears cache segment
    /// <inheritdoc />
    internal override void ClearCache(int fromIndex, int toIndex)
    {
        // delete and deliver instruction in reverse
        // order to prevent recursive recompositions

        for (int i = toIndex; i >= fromIndex; i--)
        {
            TSeries r = Cache[i];
            Act act = ModifyCache(Act.Delete, r);
            NotifyObservers(act, r);
        }
    }

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
