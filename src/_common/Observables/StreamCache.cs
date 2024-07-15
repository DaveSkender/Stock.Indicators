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
        index = GetIndex(timestamp, true);
        return index != -1;
    }

    // get the cache index based on item equality
    /// <inheritdoc/>
    public int GetIndex(TSeries cachedItem, bool noException)
    {
        int low = 0;
        int high = Cache.Count - 1;
        int firstMatchIndex = -1; // index of the first timestamp match

        while (low <= high)
        {
            int mid = low + ((high - low) / 2);
            int comparison = Cache[mid].Timestamp.CompareTo(cachedItem.Timestamp);

            if (comparison == 0)
            {
                // found a match by Timestamp,
                // store the index of the first match
                if (firstMatchIndex == -1)
                {
                    firstMatchIndex = mid;
                }

                // verify with Equals for an exact match
                if (Cache[mid].Equals(cachedItem))
                {
                    return mid; // exact match found
                }

                // continue searching to the left for
                // the first occurrence
                high = mid - 1;
            }
            else if (comparison < 0)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        // If a timestamp match was found but no exact
        // match, try to find an exact match in the range
        // of duplicate timestamps (e.g. Renko bricks),
        // biased towards later duplicats.
        if (firstMatchIndex != -1)
        {
            // Find the last occurrence of the matching timestamp
            int lastMatchIndex = firstMatchIndex;
            for (int i = firstMatchIndex + 1; i < Cache.Count && Cache[i].Timestamp == cachedItem.Timestamp; i++)
            {
                lastMatchIndex = i;
            }

            // Search for an exact match starting from the last occurrence
            for (int i = lastMatchIndex; i >= firstMatchIndex; i--)
            {
                if (Cache[i].Equals(cachedItem))
                {
                    return i; // exact match found among duplicates
                }
            }
        }

        if (noException)
        {
            return -1;
        }
        else
        {
            // not found
            throw new ArgumentException(
                "Matching source history not found.", nameof(cachedItem));
        }
    }

    // get the cache index based on a timestamp
    /// <inheritdoc/>
    public int GetIndex(DateTime timestamp, bool noException)
    {
        int low = 0;
        int high = Cache.Count - 1;

        while (low <= high)
        {
            int mid = low + ((high - low) / 2);
            DateTime midTimestamp = Cache[mid].Timestamp;

            if (midTimestamp == timestamp)
            {
                return mid;
            }
            else if (midTimestamp < timestamp)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        if (noException)
        {
            return -1;
        }
        else
        {
            // not found
            throw new ArgumentException(
                "Matching source history not found.", nameof(timestamp));
        }
    }

    // get first cache index at or greater than timestamp
    /// <inheritdoc/>
    public int GetInsertIndex(DateTime timestamp)
    {
        int low = 0;
        int high = Cache.Count;
        while (low < high)
        {
            int mid = low + ((high - low) / 2);
            if (Cache[mid].Timestamp < timestamp)
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }

        // At this point, low is the index of the first
        // element that is greater than or equal to timestamp
        // or Cache.Count if all elements are less than timestamp.
        // If low is equal to Cache.Count, it means there are
        // no elements greater than or equal to timestamp.
        return low < Cache.Count ? low : -1;
    }

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
        // check overflow
        if (CheckOverflow(item) is Act.DoNothing)
        {
            // duplicate found
            return Act.DoNothing;
        }

        // DETERMINE ACTion INSTRUCTION

        Act act;
        int length = Cache.Count;
        int? index = null;

        // first
        if (length == 0)
        {
            act = Act.AddNew;
            return Modify(act, item, index);
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
            index = GetIndex(item.Timestamp, true);

            // replace duplicate
            if (index >= 0)
            {
                act = Act.Update;
            }
            else
            {
                act = Act.AddOld;
                index = null;
            }
        }

        // perform actual modification, return final action
        return Modify(act, item, index);
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
    /// <param name="index">Index, if already known (optional)</param>
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
    protected Act Modify(Act act, TSeries item, int? index)
    {
        // execute action
        switch (act)
        {
            case Act.AddNew:

                Cache.Add(item);

                break;

            case Act.AddOld:

                // find
                int ao = index ?? GetInsertIndex(item.Timestamp);

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
                int uo = index ?? GetIndex(item.Timestamp, false);

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
                int d = index ?? GetIndex(item.Timestamp, false);

                // delete
                Cache.RemoveAt(d);

                break;

            case Act.DoNothing:

                break;

            case Act.Unknown:

                return Modify(item);

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
    protected Act Purge(TSeries item)
    {
        // check format and overflow
        if (CheckOverflow(item) is Act.DoNothing)
        {
            return Act.DoNothing;
        }

        // find position
        int index = GetIndex(item, true);

        if (index < 0)
        {
            // not found
            return Act.DoNothing;
        }

        // delete
        return Modify(Act.Delete, item, index);
    }

    /// <summary>
    /// DELETE item from cache at index position,
    /// without validating or analyzing the action
    /// </summary>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Index is out of range (not found).
    /// </exception>
    protected Act Purge(int index)
    {
        if (index < 0 || index >= Cache.Count)
        {
            // not found
            return Act.DoNothing;
        }

        TSeries item = Cache[index];

        // delete
        return Modify(Act.Delete, item, index);
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
