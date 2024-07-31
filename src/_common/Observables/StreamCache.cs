namespace Skender.Stock.Indicators;

// STREAM CACHE (BASE)

/// <inheritdoc cref="IStreamCache{TSeries}"/>
public abstract partial class StreamCache<TSeries>
    : IStreamCache<TSeries>
    where TSeries : ISeries
{
    #region PROPERTIES

    /// <inheritdoc/>
    public IReadOnlyList<TSeries> Results => Cache;

    /// <inheritdoc/>
    public bool IsFaulted { get; private set; }

    /// <summary>
    /// Cache of stored values (base).
    /// </summary>
    internal List<TSeries> Cache { get; private set; } = [];

    /// <summary>
    /// Most recent item saved to cache.
    /// </summary>
    internal TSeries? LastItem { get; private set; }

    /// <summary>
    /// Current count of repeated caching attempts.
    /// An overflow condition is triggered after 100.
    /// </summary>
    internal byte OverflowCount { get; private set; }
    #endregion

    // CACHE MODIFICATION

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
    private protected Act Analyze(TSeries item)
    {
        // check overflow/duplicates
        if (CheckOverflow(item) is Act.Ignore)
        {
            // duplicate found
            return Act.Ignore;
        }

        // get caching instruction
        return CheckSequence(item.Timestamp);
    }

    /// <summary>
    /// Quick check to determine if date implies
    /// whether we'll add or need to rebuild the cache.
    /// </summary>
    /// <param name="timestamp">Date to evaluate</param>
    /// <returns cref="Act">Action to take</returns>
    private protected Act CheckSequence(DateTime timestamp)
    {
        // first
        if (Cache.Count == 0)
        {
            return Act.Add;
        }

        // new (or rebuild if old)
        return timestamp > Cache[^1].Timestamp ? Act.Add : Act.Rebuild;
    }

    /// <summary>
    /// Adds item to cache and resets overflow counter.
    /// </summary>
    /// <remarks>
    /// Since this does not analyze the action, it is not
    /// recommended for use outside of the cache management system.
    /// For example, it will not prevent duplicates or overflow.
    /// </remarks>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns name="act" cref="Act">Caching action taken</returns>
    /// <exception cref="InvalidOperationException">
    /// A non-add action was attempted.
    /// </exception>
    private protected Act TryAdd(TSeries item)
    {
        Act act = Analyze(item);

        if (act is Act.Add)
        {
            Cache.Add(item);
            IsFaulted = false;
        }

        return act;
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
    private protected Act? CheckOverflow(TSeries item)
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
                // to prevent propogation
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
