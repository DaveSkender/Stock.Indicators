namespace Skender.Stock.Indicators;

// CACHE PROVIDER

/// <summary>
/// Base cache and management utilities
/// </summary>
public abstract class AbstractCache<TSeries> : IStreamCache<TSeries>
    where TSeries : struct, ISeries
{
    /// <summary>
    /// Default. Use internal cache.
    /// </summary>
    private protected AbstractCache()
    {
        Cache = [];
    }

    public IReadOnlyList<TSeries> Results => Cache;

    public bool IsFaulted { get; private set; }

    internal List<TSeries> Cache { get; }

    private TSeries LastArrival { get; set; }

    private int OverflowCount { get; set; }


    #region METHODS (CACHE MANAGEMENT)

    /// <summary>
    /// Analyze new arrival to determine caching instruction;
    /// then follow-on with caching action.
    /// </summary>
    /// <param name="item">
    ///   Fully formed cacheable time-series object.
    /// </param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OverflowException"></exception>
    protected Act CacheWithAnalysis(TSeries item)
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
            return ModifyCache(act, item);
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

        // perform actual update, return final action
        return ModifyCache(act, item);
    }

    /// <summary>
    /// Analyze and DELETE new arrivals from cache,
    /// after validating best instruction.
    /// </summary>
    /// <param name="item">
    ///   Fully formed cacheable time-series object.
    /// </param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OverflowException"></exception>
    protected Act PurgeWithAnalysis(TSeries item)
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

        TSeries t = Cache[foundIndex];

        // delete if full match
        return t.Equals(item)
            ? ModifyCache(Act.Delete, t)
            : Act.DoNothing;
    }

    /// <summary>
    /// Update cache, per "act" instruction.
    /// </summary>
    /// <param name="act" cref="Act">Caching instruction</param>
    /// <param name="item">
    ///   Fully formed cacheable time-series object.
    /// </param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected Act ModifyCache(Act act, TSeries item)
    {
        // execute action
        switch (act)
        {
            case Act.AddNew:

                Cache.Add(item);

                break;

            case Act.AddOld:

                // find
                int ao = Cache.FindIndex(x => x.Timestamp > item.Timestamp);

                // insert
                if (ao != -1)
                {
                    Cache.Insert(ao, item);
                }

                // failure to find old back-position
                else
                {
                    Cache.Add(item);
                }

                break;

            case Act.Update:

                // find
                int uo = Cache.FindIndex(item.Timestamp);

                // does not exist
                if (uo == -1)
                {
                    throw new InvalidOperationException(
                        "Cache update target not found.");
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
                int d = Cache.FindIndex(item.Timestamp);

                // delete
                if (d != -1)
                {
                    Cache.RemoveAt(d);
                }

                // failure to find should never happen
                else
                {
                    throw new InvalidOperationException(
                        "Cache delete target not found.");
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
    /// Validate inbound item and compare to prior arrivals
    /// to manage and prevent overflow conditions.
    /// <remarks>
    /// Overflow can occur for many reasons;
    /// the most aggregious being a circular subscription,
    /// set by an external user.
    /// </remarks>
    /// </summary>
    /// <param name="item">
    ///   Fully formed cacheable time-series object.
    /// </param>
    /// <returns cref="Act">
    /// An "do nothing" act instruction if duplicate or 'null'
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OverflowException"></exception>
    private Act? CheckOverflow(TSeries item)
    {
        Act? act = null;

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

    #region METHODS (UTILITIES)

    // get the cache index based on a timestamp
    /// <inheritdoc/>
    public int FindIndex(DateTime timeStamp)
        => Cache.FindIndex(x => x.Timestamp == timeStamp);

    // clear entire cache without restore
    /// <inheritdoc/>
    public void ClearCache() => ClearCache(0);

    /// <inheritdoc/>
    public void ClearCache(DateTime fromTimestamp)
    {
        // start of range
        int fromIndex = Cache
            .FindIndex(c => c.Timestamp >= fromTimestamp);

        // something to do
        if (fromIndex != -1)
        {
            ClearCache(fromIndex);
        }
    }

    /// <inheritdoc/>
    public void ClearCache(int fromIndex)
        => ClearCache(fromIndex, toIndex: Cache.Count - 1);

    /// <summary>
    /// Deletes cache entries between index range values.
    /// </summary>
    /// <remarks>
    /// This is implemented in inheriting (provider) class
    /// due to unique requirement to notify subscribers.
    /// </remarks>
    /// <param name="fromIndex">First element to delete</param>
    /// <param name="toIndex">Last element to delete</param>
    protected abstract void ClearCache(
        int fromIndex, int toIndex);
    #endregion
}
