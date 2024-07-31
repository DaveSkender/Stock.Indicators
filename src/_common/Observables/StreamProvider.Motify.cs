namespace Skender.Stock.Indicators;

// STREAM PROVIDER: MODIFY CACHE + NOTIFY SUBSCRIBERS (MOTIFY)

public abstract partial class StreamProvider<TSeries>
{
    /// clear cache without restore, from timestamp
    /// <inheritdoc/>
    public void ClearCache(DateTime fromTimestamp)
    {
        // start of range
        int fromIndex = GetIndexGte(fromTimestamp);

        // clear, from index (-1 okay)
        ClearCache(fromIndex);
    }

    /// clear cache from index to end
    /// <inheritdoc/>
    public void ClearCache(int fromIndex)
    {
        // nothing to do
        if (Cache.Count == 0 || fromIndex >= Cache.Count)
        {
            return;
        }

        // clear, with time context
        DateTime timestamp = fromIndex < 0
            ? DateTime.MinValue
            : Cache[fromIndex].Timestamp;

        ClearCache(fromIndex, timestamp);
    }

    /// <summary>
    /// Clear cache without restore,
    /// from valid index and time context.
    /// </summary>
    private void ClearCache(int fromIndex, DateTime fromTimestamp)
    {
        // clear all
        if (fromIndex <= 0)
        {
            Cache.Clear();
        }

        // clear partial
        else
        {
            Cache.RemoveRange(fromIndex, Cache.Count - fromIndex);
        }

        // notify subscribers
        RebuildObservers(fromTimestamp);
    }

    /// <summary>
    /// Modify cache (attempt to add) and notify observers.
    /// </summary>
    /// <param name="result"><c>TSeries</c> item to cache</param>
    /// <param name="index">Cached index position (hint)</param>
    protected void Motify(TSeries result, int? index)
    {
        try
        {
            Act actTaken = TryAdd(result);
            NotifyObservers(actTaken, result, index);
        }
        catch (OverflowException ox)
        {
            NotifyObserversOfError(ox);
            EndTransmission();
            throw;
        }
    }

    /// <summary>
    /// Rebuilds all subscriber caches from point in time.
    /// </summary>
    /// <param name="timestamp">Rebuild starting positions</param>
    protected void RebuildObservers(DateTime timestamp)
    {
        foreach (IStreamObserver<(Act, TSeries, int?)> obs
            in _subscribers.ToArray())
        {
            obs.RebuildCache(timestamp);
        }
    }

    /// <summary>
    /// Sends <c>TSeries</c> item to all subscribers
    /// </summary>
    /// <param name="act" cref="Act">Caching instruction</param>
    /// <param name="item"><c>TSeries</c> item to send</param>
    /// <param name="index">Provider index hint</param>
    private void NotifyObservers(Act act, TSeries? item, int? index)
    {
        // do not propogate "do nothing" acts
        if (act == Act.Ignore || item is null)
        {
            return;
        }

        // send to subscribers
        foreach (IStreamObserver<(Act, TSeries, int?)> obs
            in _subscribers.ToArray())
        {
            obs.OnNext((act, item, index));
        }
    }

    /// <summary>
    /// Sends error (exception) to all subscribers
    /// </summary>
    /// <param name="exception"></param>
    private void NotifyObserversOfError(Exception exception)
    {
        // send to subscribers
        foreach (IStreamObserver<(Act, TSeries, int?)> obs
            in _subscribers.ToArray())
        {
            obs.OnError(exception);
        }
    }
}
