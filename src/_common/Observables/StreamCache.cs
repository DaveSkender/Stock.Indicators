namespace Skender.Stock.Indicators;

// STREAMING CACHE STORE

/// <inheritdoc cref="IStreamCache{TSeries}"/>
public abstract class StreamCache<TSeries>
    : IStreamCache<TSeries>
    where TSeries : ISeries
{
    /// <inheritdoc/>
    public IReadOnlyList<TSeries> Results => Cache;

    /// <inheritdoc/>
    public bool IsFaulted { get; private set; }

    /// <summary>
    /// Cache of stored values (base).
    /// </summary>
    internal List<TSeries> Cache { get; private set; } = [];

    /// <summary>
    /// Most recent arrival to cache.
    /// </summary>
    internal TSeries? LastArrival { get; private set; }

    /// <summary>
    /// Current count of repeated arrivals.
    /// An overflow condition is triggered after 100.
    /// </summary>
    internal byte OverflowCount { get; private set; }

    #region METHODS (CACHE UTILITIES)

    // reset fault flag and condition
    /// <inheritdoc/>
    public void ResetFault()
    {
        OverflowCount = 0;
        IsFaulted = false;
    }

    // try/get the cache index based on a timestamp
    /// <inheritdoc/>
    public bool TryFindIndex(DateTime timestamp, out int index)
    {
        index = Cache.FindIndex(x => x.Timestamp == timestamp);
        return index != -1;
    }

    // get the cache index based on item equality
    /// <inheritdoc/>
    public int ExactIndex(TSeries cachedItem)
    {
        int index = Cache.FindIndex(c => c.Equals(cachedItem));

        // source unexpectedly not found
        return index == -1
            ? throw new ArgumentException(
                "Matching source history not found.", nameof(cachedItem))
            : index;
    }

    // get the cache index based on a timestamp
    /// <inheritdoc/>
    public int ExactIndex(DateTime timestamp)
    {
        int index = Cache.FindIndex(
            c => c.Timestamp == timestamp);

        // source unexpectedly not found
        return index == -1
            ? throw new ArgumentException(
                "Matching source history not found.", nameof(timestamp))
            : index;
    }

    // get the cache index based on a timestamp
    /// <inheritdoc/>
    public int FromIndex(DateTime timestamp)
        => Cache.FindIndex(c => c.Timestamp >= timestamp);

    #endregion

    #region METHODS (CACHE MODIFICATION)

    // clear cache without restore
    /// <inheritdoc/>
    public void ClearCache() => ClearCache(0);
    public abstract void ClearCache(DateTime fromTimestamp);
    public abstract void ClearCache(int fromIndex);

    /// <summary>
    /// Analyze new arrival to determine caching instruction;
    /// then follow-on with caching action.
    /// </summary>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentException">
    /// Item to modify is not found.
    /// </exception>
    /// <exception cref="OverflowException">
    /// Too many sequential duplicates were detected.
    /// </exception>
    internal Act Modify(TSeries item)
    {
        // check format and overflow
        if (CheckOverflow(item) is Act.DoNothing)
        {
            return Act.DoNothing;
        }


        // DETERMINE ACTion INSTRUCTION

        Act act;
        int length = Cache.Count;

        // first
        if (length == 0)
        {
            act = Act.AddNew;
            return Modify(act, item);
        }

        TSeries last = Cache[length - 1];

        // newer
        if (item.Timestamp > last.Timestamp)
        {
            act = Act.AddNew;
        }

        // repeat or late arrival
        else
        {
            // seek duplicate
            int foundIndex = Cache
                .FindIndex(x => x.Timestamp == item.Timestamp);

            // replace duplicate
            act = foundIndex == -1 ? Act.AddOld : Act.Update;
        }

        // perform actual modification, return final action
        return Modify(act, item);
    }

    /// <summary>
    /// Update cache, per "act" instruction, without analysis.
    /// </summary>
    /// <remarks>
    /// Since this does not analyze the action, it is not
    /// recommended for use outside of the cache management system.
    /// For example, it will not prevent duplicates or overflow.
    /// </remarks>
    /// <param name="act" cref="Act">Caching instruction</param>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentException">
    /// Item to modify is not found.
    /// </exception>
    /// <exception cref="OverflowException">
    /// Too many sequential duplicates were detected.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Action type is unknown.
    /// </exception>
    protected Act Modify(Act act, TSeries item)
    {
        // execute action
        switch (act)
        {
            case Act.AddNew:

                Cache.Add(item);

                break;

            case Act.AddOld:

                // find
                int ao = Cache
                    .FindIndex(c => c.Timestamp > item.Timestamp);

                // insert
                if (ao != -1)
                {
                    Cache.Insert(ao, item);
                }

                // failure to find newer index
                else
                {
                    Cache.Add(item);
                }

                break;

            case Act.Update:

                // find
                int uo = Cache
                    .FindIndex(c => c.Timestamp == item.Timestamp);

                // does not exist
                if (uo == -1)
                {
                    throw new ArgumentException(
                        "Cache update target not found.", nameof(item));
                }

                // duplicate
                if (item.Equals(Cache[uo]))
                {
                    return Act.DoNothing;
                }

                // replace
                Cache[uo] = item;

                break;

            case Act.Delete:

                // find
                int d = Cache
                    .FindIndex(c => c.Timestamp == item.Timestamp);  // TODO: this is redundant (above)

                // delete
                if (d != -1)
                {
                    Cache.RemoveAt(d);
                }

                // failure to find should never happen
                else
                {
                    throw new ArgumentException(
                        "Cache delete target not found.", nameof(item));
                }

                break;

            case Act.DoNothing:

                break;

            // should never get here
            default:

                throw new InvalidOperationException(
                    "Undefined cache action.");
        }

        IsFaulted = false;
        return act;
    }

    /// <summary>
    /// Analyze and DELETE new arrivals from cache,
    /// after validating best instruction.
    /// </summary>
    /// <param name="item">Cacheable time-series object</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="OverflowException">
    /// Too many sequential duplicates were detected.
    /// </exception>
    protected Act Purge(TSeries item)  // TODO: add index option
    {
        // check format and overflow
        if (CheckOverflow(item) is Act.DoNothing)
        {
            return Act.DoNothing;
        }

        // determine if record exists
        int foundIndex = Cache
            .FindIndex(x => x.Timestamp == item.Timestamp);

        // not found
        if (foundIndex == -1)
        {
            return Act.DoNothing;
        }

        TSeries found = Cache[foundIndex];

        // delete if full match
        return found.Equals(item)
            ? Modify(Act.Delete, found)
            : Act.DoNothing;
    }

    /// <summary>
    /// Validate inbound item and compare to prior arrivals
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
    private Act? CheckOverflow(TSeries item)
    {
        Act? act = null;

        // skip first arrival
        if (LastArrival is null)
        {
            LastArrival = item;
            return act;
        }

        // check for overflow condition
        if (item.Timestamp == LastArrival.Timestamp)
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
            // TODO: not handling add-back after delete, registers as dup
            if (item.Equals(LastArrival))
            {
                // to prevent propogation
                // of identical cache entry
                act = Act.DoNothing;
            }

            // same date with different values
            // continues as an update
            else
            {
                LastArrival = item;
            }
        }
        else
        {
            OverflowCount = 0;
            LastArrival = item;
        }

        return act;
    }
    #endregion
}
