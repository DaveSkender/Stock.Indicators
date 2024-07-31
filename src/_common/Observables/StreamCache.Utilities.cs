namespace Skender.Stock.Indicators;

// STREAM CACHE (UTILITIES)

/// <inheritdoc cref="IStreamCache{TSeries}"/>
public abstract partial class StreamCache<TSeries>
    : IStreamCache<TSeries>
    where TSeries : ISeries
{
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
    public int GetIndex(TSeries cachedItem, bool throwOnFail)
    {
        int low = 0;
        int high = Cache.Count - 1;
        int firstMatchIndex = -1;

        while (low <= high)
        {
            int mid = low + ((high - low) / 2);
            int comparison = Cache[mid].Timestamp.CompareTo(cachedItem.Timestamp);

            if (comparison == 0)
            {
                // Found a match by Timestamp,
                // store the index of the first match
                if (firstMatchIndex == -1)
                {
                    firstMatchIndex = mid;
                }

                // Verify with Equals for an exact match
                if (Cache[mid].Equals(cachedItem))
                {
                    return mid; // exact match found
                }

                // Continue searching to the left for
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

        // not found
        return throwOnFail
            ? throw new ArgumentException(
                "Matching source history not found.", nameof(cachedItem))
            : -1;
    }

    // get the cache index based on a timestamp
    /// <inheritdoc/>
    public int GetIndex(DateTime timestamp, bool throwOnFail)
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

        // not found
        return throwOnFail
            ? throw new ArgumentException(
                "Matching source history not found.", nameof(timestamp))
            : -1;
    }

    // get first cache index at or greater than timestamp
    /// <inheritdoc/>
    public int GetIndexGte(DateTime timestamp)
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
}
