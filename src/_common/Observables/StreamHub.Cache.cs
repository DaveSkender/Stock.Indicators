namespace Skender.Stock.Indicators;

// STREAM HUB (CACHE)

public abstract partial class StreamHub<TIn, TOut> : IStreamCache<TOut>
{
    #region PROPERTIES

    /// <inheritdoc/>
    public IReadOnlyList<TOut> Results => Cache;

    /// <inheritdoc/>
    public bool IsFaulted { get; private set; }

    /// <summary>
    /// Cache of stored values (base).
    /// </summary>
    internal List<TOut> Cache { get; } = [];

    /// <summary>
    /// Current count of repeated caching attempts.
    /// An overflow condition is triggered after 100.
    /// </summary>
    internal byte OverflowCount { get; private set; }

    /// <summary>
    /// Most recent item saved to cache.
    /// </summary>
    private TOut? LastItem { get; set; }

    #endregion

    // CACHE MODIFICATION

    /// <summary>
    /// Adds item to cache.
    /// </summary>
    /// <remarks>
    /// When the cache management system cannot add the item
    /// to the cache due to an overflow condition or other issue,
    /// the caller will be given an alternate instruction.
    /// </remarks>
    /// <param name="item">Time-series object to cache</param>
    /// <returns name="act" cref="Act">Caching action</returns>
    private Act TryAdd(TOut item)
    {
        Act act = Analyze(item);

        // return alternate instruction
        if (act is not Act.Add)
        {
            return act;
        }

        // add to cache
        Cache.Add(item);
        IsFaulted = false;

        return act;
    }

    /// <summary>
    /// Analyze cache candidate to determine caching instruction.
    /// </summary>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns cref="Act">Action to take</returns>
    /// <exception cref="ArgumentException">
    /// Item to modify is not found.
    /// </exception>
    /// <exception cref="OverflowException">
    /// Too many sequential duplicates were detected.
    /// </exception>
    private Act Analyze(TOut item)
    {
        // check overflow/duplicates
        if (CheckOverflow(item) is Act.Ignore)
        {
            // duplicate found
            return Act.Ignore;
        }

        // consider late-arrival (need to rebuild)
        return Cache.Count == 0 || item.Timestamp > Cache[^1].Timestamp
            ? Act.Add
            : Act.Rebuild;
    }

    /// <summary>
    /// Validate outbound item and compare to prior sent item,
    /// to gracefully manage and prevent overflow conditions.
    /// </summary>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns cref="Act">
    /// A "do nothing" act instruction if duplicate or 'null'
    /// when no overflow condition is detected.
    /// </returns>
    /// <exception cref="OverflowException">
    /// Too many sequential duplicates were detected.
    /// </exception>
    private Act? CheckOverflow(TOut item)
    {
        Act? act = null;

        // skip first arrival
        if (LastItem is null)
        {
            LastItem = item;
            return act;
        }

        // check for overflow condition
        if (item.Timestamp == LastItem.Timestamp)
        {
            // note: we have a better IsEqual() comparison method below,
            // but it is too expensive as an initial quick evaluation.

            OverflowCount++;

            if (OverflowCount > 100)
            {
                const string msg = """
                   A repeated stream update exceeded the 100 attempt threshold.
                   Check and remove circular chains or check your stream provider.
                   Provider terminated.
                   """;

                IsFaulted = true;

                throw new OverflowException(msg);

                // note: overflow exception is also further handled by providers,
                // where it will EndTransmission(); and then throw error to user.
            }

            // aggressive property value comparison
            if (item.Equals(LastItem))
            {
                // to prevent propagation
                // of identical cache entry
                act = Act.Ignore;
            }

            // same date with different values
            // continues as an update
            else
            {
                LastItem = item;
            }
        }
        else
        {
            OverflowCount = 0;
            LastItem = item;
        }

        return act;
    }
}
