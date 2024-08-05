
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
        ClearCache();
        Subscription = Provider.Subscribe(this);
        RebuildCache();
    }

    // rebuild cache
    /// <inheritdoc/>
    public void RebuildCache() => RebuildCache(0);

    // rebuild cache from timestamp
    /// <inheritdoc/>
    public void RebuildCache(
        DateTime fromTimestamp)
    {
        int fromIndex = GetInsertIndex(fromTimestamp);

        // nothing to rebuild
        if (fromIndex < 0)
        {
            return;
        }

        int provIndex = Provider.GetInsertIndex(fromTimestamp);

        // nothing to restore
        if (provIndex < 0)
        {
            provIndex = int.MaxValue;
        }

        RebuildCache(fromIndex, provIndex);
    }

    // rebuild cache from index
    /// <inheritdoc/>
    public void RebuildCache(int fromIndex)
    {
        // get equivalent provider index
        int provIndex;

        if (fromIndex <= 0)
        {
            provIndex = 0;
        }
        else
        {
            TOut item = Cache[fromIndex];

            provIndex = Provider.GetInsertIndex(item.Timestamp);

            if (provIndex < 0)
            {
                // nothing to restore
                provIndex = int.MaxValue;
            }
        }

        RebuildCache(fromIndex, provIndex);
    }

    /// <summary>
    /// Rebuild cache from index and provider index positions.
    /// </summary>
    /// <param name="thisIndex">Cache starting position to purge.</param>
    /// <param name="provIndex">Provider starting position to add back.</param>
    internal void RebuildCache(int thisIndex, int provIndex)
    {
        // clear outdated cache
        ClearCache(thisIndex);

        // rebuild cache from provider
        for (int i = provIndex; i < Provider.Results.Count; i++)
        {
            OnNext((Act.AddNew, Provider.Results[i], i));
        }
    }
}
