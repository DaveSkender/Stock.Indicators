
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
    public void RebuildCache(DateTime fromTimestamp)
    {
        int fromIndex = Cache.GetIndexGte(fromTimestamp);

        // nothing to rebuild
        if (fromIndex < 0)
        {
            return;
        }

        int provIndex = ProviderCache.GetIndexGte(fromTimestamp);
        RebuildCache(fromIndex, provIndex);
    }

    // rebuild cache from index
    /// <inheritdoc/>
    public void RebuildCache(int fromIndex, int? provIndex = null)
    {
        // nothing to do
        if (fromIndex >= Cache.Count)
        {
            return;
        }

        DateTime timestamp;

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
            provIndex ??= ProviderCache.GetIndexGte(timestamp);
            Cache.RemoveRange(fromIndex, Cache.Count - fromIndex);
        }

        // rebuild cache from provider
        if (provIndex >= 0)
        {
            for (int i = (int)provIndex; i < ProviderCache.Count; i++)
            {
                OnNextAddition(ProviderCache[i], i);
            }
        }

        // rebuild observers
        RebuildObservers(timestamp);
    }

    /// <summary>
    /// Rebuilds all subscriber caches from point in time.
    /// </summary>
    /// <param name="fromTimestamp">Rebuild starting positions</param>
    private void RebuildObservers(DateTime fromTimestamp)
    {
        foreach (IStreamObserver<TOut> obs
                 in _observers.ToArray())
        {
            obs.RebuildCache(fromTimestamp);
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
    public IReadOnlyList<TOut> GetCacheRef() => Cache;


    /// <summary>
    /// Clears the cache from a point in time and
    /// cascades the removal of cache entries to all subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// The timestamp from which to start removing cache entries.
    /// </param>
    public void CascadeCacheRemoval(DateTime fromTimestamp)
    {
        ClearCache(fromTimestamp);

        foreach (IStreamObserver<TOut> observer
            in _observers.ToArray())
        {
            observer.OnCacheRemoval(fromTimestamp);
        }
    }

    /// <summary>
    /// Cascades the removal of cache entries to all subscribers.
    /// </summary>
    /// <param name="fromIndex">
    /// The index from which to start removing cache entries.
    /// </param>
    public void CascadeCacheRemoval(int fromIndex)
    {
        DateTime fromTimestamp = fromIndex <= 0
            ? DateTime.MinValue
            : Cache[fromIndex].Timestamp;

        CascadeCacheRemoval(fromTimestamp);
    }

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
