
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
        int fromIndex = GetIndexGte(fromTimestamp);

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
            provIndex = Provider.GetIndexGte(timestamp);
            Cache.RemoveRange(fromIndex, Cache.Count - fromIndex);
        }

        // rebuild cache from provider
        if (provIndex >= 0)
        {
            for (int i = provIndex; i < Provider.Results.Count; i++)
            {
                Add(Provider.Results[i], i);
            }
        }

        // rebuild observers
        RebuildObservers(timestamp);
    }
}
