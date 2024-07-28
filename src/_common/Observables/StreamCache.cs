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
    /// Most recent arrival to cache.
    /// </summary>
    internal TSeries? LastArrival { get; private set; }

    /// <summary>
    /// Current count of repeated arrivals.
    /// An overflow condition is triggered after 100.
    /// </summary>
    internal byte OverflowCount { get; private set; }
    #endregion

    // CACHE MODIFICATION

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
}
