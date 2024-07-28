namespace Skender.Stock.Indicators;

// STREAM PROVIDER: MODIFY CACHE + NOTIFY SUBSCRIBERS (MOTIFY)

public abstract partial class StreamProvider<TSeries>
{
    /// clear cache without restore, from timestamp
    /// <inheritdoc/>
    public override void ClearCache(DateTime fromTimestamp)
    {
        // start of range
        int fromIndex = GetInsertIndex(fromTimestamp);

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
    /// Modify cache and notify observers.
    /// </summary>
    /// <param name="act" cref="Act">Caching instruction</param>
    /// <param name="result"><c>TSeries</c> item to send</param>
    /// <param name="index">Cached index position</param>
    protected void Motify(Act act, TSeries result, int? index)
    {
        Act actTaken = Modify(act, result, index);
        NotifyObservers(actTaken, result, index);
    }

    /// <summary>
    /// Sends <c>TSeries</c> item to all subscribers
    /// </summary>
    /// <param name="act" cref="Act">Caching instruction</param>
    /// <param name="item"><c>TSeries</c> item to send</param>
    /// <param name="index">Provider index hint</param>
    protected void NotifyObservers(Act act, TSeries? item, int? index)
    {
        // do not propogate "do nothing" acts
        if (act == Act.DoNothing || item is null)
        {
            return;
        }

        // send to subscribers
        foreach (IObserver<(Act, TSeries, int?)> obs
            in _subscribers.ToArray())
        {
            obs.OnNext((act, item, index));
        }
    }

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
            Motify(Act.Delete, Cache[i], i);
        }
    }
}
