namespace Skender.Stock.Indicators;

// CACHE PROVIDER

/// <summary>
/// Base cache and management utilities
/// </summary>
public abstract class AbstractCache<TSeries> : IStreamCache<TSeries>
    where TSeries : struct, ISeries
{
    // CONSTRUCTORS

    /// <summary>
    /// Default. Use internal cache.
    /// </summary>
    internal AbstractCache()
    {
        Cache = [];
    }

    /// <summary>
    /// Optional.  Use externally provided cache.
    /// </summary>
    /// <param name="externalCache"></param>
    /// <remarks>
    /// DO NOT USE.  This is for future consideration only,
    /// to allow users to provide their own cache storage location.
    /// </remarks>
    internal AbstractCache(
        List<TSeries> externalCache)
    {
        Cache = externalCache;
        throw new NotImplementedException();
    }

    // PROPERTIES

    public IReadOnlyList<TSeries> Results => Cache;

    public bool IsFaulted { get; internal set; }

    internal List<TSeries> Cache { get; set; }

    internal TSeries LastArrival { get; set; } = new();

    internal int OverflowCount { get; set; }


    // METHODS

    // get a segment of the cache
    /// <inheritdoc/>
    public IReadOnlyList<TSeries> GetRange(int index, int count)
        => Cache.GetRange(index, count);

    // get the cache index based on a timestamp
    /// <inheritdoc/>
    public int FindIndex(DateTime timeStamp)
        => Cache.FindIndex(x => x.Timestamp == timeStamp);

    // clear entire cache without restore
    /// <inheritdoc/>
    public void ClearCache()
    {
        // nothing to do
        if (Cache.Count == 0)
        {
            Cache = [];
            return;
        }

        // reset all (with handling)
        ClearCache(0);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    /// `fromTimestamp` not found in cache
    /// </exception>
    public void ClearCache(DateTime fromTimestamp)
    {
        int s = Cache.FindIndex(fromTimestamp);  // start of range

        if (s == -1)
        {
            throw new InvalidOperationException(
                "Cache clear starting target not found"
              + " for provided timestamp.");
        }

        ClearCache(s);
    }

    /// <summary>
    /// Deletes all cache entries after `fromIndex` (inclusive)
    /// </summary>
    /// <param name="fromIndex">From index, inclusive</param>
    internal void ClearCache(int fromIndex)
        => ClearCache(fromIndex, toIndex: Math.Max(0, Cache.Count - 1));

    /// <summary>
    /// Deletes cache entries between index range values.
    /// </summary>
    /// <remarks>
    /// This is usually overridden/implemented in inheriting
    /// classes due to unique requirements to notify subscribers.
    /// </remarks>
    /// <param name="fromIndex">First element to delete</param>
    /// <param name="toIndex">Last element to delete</param>
    internal virtual void ClearCache(int fromIndex, int toIndex)
    {
        for (int i = toIndex; i >= fromIndex; i--)
        {
            TSeries r = Cache[i];
            ModifyCache(Act.Delete, r);
        }
    }

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
    internal Act CacheWithAnalysis(TSeries item)
    {
        // Currently, only inbound Quote is accepted as an
        // external chain entry point and is the only type
        // using this method.   -- DS 12/4/2023

        // TODO: consider moving analysis to QuoteProvider,
        // if it's the only user.

        // check format and overflow
        if (CheckOverflow(item) is Act.DoNothing)
        {
            return Act.DoNothing;
        };


        // DETERMINE ACTion INSTRUCTION

        Act act;
        List<TSeries> cache = Cache;
        int length = cache.Count;

        // first
        if (length == 0)
        {
            act = Act.AddNew;
            return ModifyCache(act, item);
        }

        TSeries last = cache[length - 1];

        // newer
        if (item.Timestamp > last.Timestamp)
        {
            act = Act.AddNew;
        }

        // repeat or late arrival
        else
        {
            // seek duplicate
            int foundIndex = cache
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
    internal Act PurgeWithAnalysis(TSeries item)
    {
        // check format and overflow
        if (CheckOverflow(item) is Act.DoNothing)
        {
            return Act.DoNothing;
        };

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
    internal Act ModifyCache(Act act, TSeries item)
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

                // failure to find should never happen
                else
                {
                    throw new InvalidOperationException(
                        "Cache insert target not found.");
                }

                break;

            case Act.Update:

                // find
                int uo = Cache.FindIndex(item.Timestamp);

                // replace
                Cache[uo] = uo != -1
                    ? item
                    : throw new InvalidOperationException(
                        "Cache update target not found.");

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
                string msg = """
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
}