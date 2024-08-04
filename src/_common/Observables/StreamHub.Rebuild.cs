
namespace Skender.Stock.Indicators;

// STREAM HUB (REBUILD CACHE)

public abstract partial class StreamHub<TIn, TOut>
{
    // full reset
    /// <inheritdoc/>
    public void Reinitialize()
    {
        Unsubscribe();
        ResetFault();
        RebuildCache();
        Subscription = Provider.Subscribe(this);

        // TODO: make reinitialization abstract,
        // and build initial Cache from faster static method

        // TODO: evaluate race condition between rebuild
        // and subscribe; will it miss any high frequency data?
    }

    // rebuild cache
    /// <inheritdoc/>
    public void RebuildCache()
    {
        Cache.Clear();
        RebuildCache(-1);
    }

    // rebuild cache from timestamp
    /// <inheritdoc/>
    public void RebuildCache(
        DateTime fromTimestamp)
    {
        int fromIndex = Cache.GetIndexGte(fromTimestamp);

        // nothing to rebuild
        if (fromIndex < 0)
        {
            return;
        }

        RebuildCache(fromIndex);
    }

    // rebuild cache from index
    /// <inheritdoc/>
    public void RebuildCache(int fromIndex)
    {
        // nothing to do
        if (fromIndex >= Cache.Count)
        {
            return;
        }

        DateTime timestamp;
        int provIndex;

        // full rebuild scenario
        if (fromIndex <= 0 || Cache.Count is 0)
        {
            timestamp = DateTime.MinValue;
            provIndex = 0;
            Cache.Clear();
        }

        // partial rebuild scenario
        else
        {
            timestamp = Cache[fromIndex].Timestamp;
            provIndex = ProviderCache.GetIndexGte(timestamp);
            Cache.RemoveRange(fromIndex, Cache.Count - fromIndex);
        }

        // rebuild cache from provider
        if (provIndex >= 0)
        {
            for (int i = provIndex; i < ProviderCache.Count; i++)
            {
                Add(ProviderCache[i], i);
            }
        }

        // rebuild observers
        RebuildObservers(timestamp);
    }

    /// <summary>
    /// Rebuilds all subscriber caches from point in time.
    /// </summary>
    /// <param name="timestamp">Rebuild starting positions</param>
    private void RebuildObservers(DateTime timestamp)
    {
        foreach (IStreamObserver<TOut> obs
                 in _observers.ToArray())
        {
            obs.RebuildCache(timestamp);
        }
    }

    // reset fault flag and condition
    /// <inheritdoc/>
    public void ResetFault()
    {
        OverflowCount = 0;
        IsFaulted = false;
    }

    // fetch cache reference
    /// <inheritdoc/>
    public IReadOnlyList<TOut> GetReadOnlyCache() => Cache;

    /// clear cache without restore, from timestamp.
    /// <inheritdoc/>
    public void ClearCache(DateTime fromTimestamp)
    {
        // start of range
        int fromIndex = Cache.GetIndexGte(fromTimestamp);

        // clear, from index (-1 okay)
        ClearCache(fromIndex);
    }

    /// clear cache without restore, from index.
    /// <inheritdoc/>
    public void ClearCache(int fromIndex)
    {
        // nothing to do
        if (Cache.Count == 0 || fromIndex >= Cache.Count)
        {
            return;
        }

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
    }
}
