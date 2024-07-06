using System.Runtime.InteropServices;

namespace Skender.Stock.Indicators;

// CACHE STORE

/// <summary>
/// Base cache and management utilities
/// </summary>
public class StreamCache<TSeries> : IStreamCache
    where TSeries : struct, ISeries
{
    /// <summary>
    /// Default. Use internal cache.
    /// </summary>
    internal StreamCache()
    {
        Cache = [];
    }

    /// <summary>
    /// The cache failed and is no longer operational.
    /// </summary>
    /// <remarks>
    /// The cache and its provider will no longer function.
    /// Use <see cref="IStreamCache.Reset()"/> to remove this flag.
    /// This primarily occurs when there is an overflow condition.
    /// This usually occurs when a circular chain is detected,
    /// or when there were too many sequential duplicates.
    /// </remarks>
    public bool IsFaulted { get; private set; }

    /// <summary>
    /// Cache of stored values (base).
    /// </summary>
    /// <remarks>
    /// Only access this <c>Cache</c> through implemented
    /// methods. It's also okay to use indexing operations
    /// like <see cref="List{T}.FindIndex(Predicate{T})"/>
    /// and <see cref="List{T}.Count"/> since they do not
    /// copy the struct.  Use <see cref="ReadCache"/> for
    /// referencing objects in the list.
    /// </remarks>
    internal List<TSeries> Cache { get; }

    /// <summary>
    /// Read-only Span-list of the stored values <see cref="Cache"/>.
    /// </summary>
    /// <remarks>
    /// Use this to referencing objects to avoid copying the struct.
    /// </remarks>
    internal ReadOnlySpan<TSeries> ReadCache
        => CollectionsMarshal.AsSpan(Cache);

    /// <summary>
    /// Most recent arrival to cache.
    /// </summary>
    internal TSeries LastArrival { get; private set; }

    /// <summary>
    /// Current count of repeated arrivals.
    /// An overflow condition is triggered after 100.
    /// </summary>
    internal byte OverflowCount { get; private set; }


    #region METHODS (CACHE MANAGER)

    public void Reset()
    {
        OverflowCount = 0;
        IsFaulted = false;
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

        TSeries last = ReadCache[length - 1];

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
    /// <param name="item">
    ///   Fully formed cacheable time-series object.
    /// </param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentException">
    /// Item to modify is not found.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Action is not defined.
    /// </exception>
    internal Act Modify(Act act, TSeries item)
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
                    .FindIndex(c => c.Timestamp == item.Timestamp);

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
    /// <param name="item">
    ///   Fully formed cacheable time-series object.
    /// </param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OverflowException"></exception>
    internal Act Purge(TSeries item)
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

        TSeries found = ReadCache[foundIndex];

        // delete if full match
        return found.Equals(item)
            ? Modify(Act.Delete, found)
            : Act.DoNothing;
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
    public bool TryFindIndex(DateTime timestamp, out int index)
    {
        index = Cache.FindIndex(x => x.Timestamp == timestamp);
        return index != -1;
    }

    /// <summary>
    /// Get the cache index based on item equality.
    /// </summary>
    /// <param name="item">
    /// Timeseries object to find
    /// </param>
    /// <returns>Index position</returns>
    /// <exception cref="ArgumentException">
    /// When items is not found (should never happen).
    /// </exception>
    internal int Position(TSeries item)
    {
        int index = Cache.FindIndex(c => c.Equals(item));

        // source unexpectedly not found
        return index == -1
            ? throw new ArgumentException(
                "Matching source history not found.", nameof(item))
            : index;
    }

    /// <summary>
    /// Get the cache index based on a timestamp.
    /// </summary>
    /// <remarks>
    /// Only use this when you are looking for a point in time
    /// without a a matching item for context.  In most cases
    /// <see cref="Position(TSeries)"/> is more appropriate.
    /// </remarks>
    /// <param name="timestamp">
    /// Timestamp of cached item
    /// </param>
    /// <returns>Index position</returns>
    /// <exception cref="ArgumentException">
    /// When timestamp is not found (should never happen).
    /// </exception>
    internal int Position(DateTime timestamp)
    {
        int index = Cache.FindIndex(
            c => c.Timestamp == timestamp);

        // source unexpectedly not found
        return index == -1
            ? throw new ArgumentException(
                "Matching source history not found.", nameof(timestamp))
            : index;
    }
    #endregion
}
