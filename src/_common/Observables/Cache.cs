namespace Skender.Stock.Indicators;

// base cache for Quotes
public abstract class QuoteCache<TQuote>
    : StreamCache<TQuote>
    where TQuote : IQuote, new()
{
    internal QuoteCache()
        : base(isChainor: false) { }

    public IEnumerable<TQuote> Quotes => Cache;
}

// base cache for Indicator results
public abstract class ResultCache<TResult>
    : StreamCache<TResult>
    where TResult : IResult, new()
{
    internal ResultCache(bool isChainor)
        : base(isChainor) { }

    public IEnumerable<TResult> Results => Cache;
}

// base result or series cache
/// <inheritdoc cref="IStreamCache{TSeris}"/>
public abstract class StreamCache<TSeries>
    : ChainProvider, IStreamCache<TSeries>
    where TSeries : ISeries, new()
{
    // fields
    private readonly bool isChainor;

    // constructor
    protected internal StreamCache(bool isChainor)
    {
        Cache = [];
        LastArrival = new();
        OverflowCount = 0;
        this.isChainor = isChainor;
    }

    // PROPERTIES

    internal List<TSeries> Cache { get; set; }

    internal TSeries LastArrival { get; set; }

    internal int OverflowCount { get; set; }

    // METHODS

    /// <inheritdoc cref="IStreamCache{TSeries}.ToString"/>
    public abstract override string ToString();

    /// <summary>
    /// Deletes all cache and chain records, gracefully
    /// </summary>
    public void ClearCache()
    {
        // nothing to do
        if (Cache.Count == 0)
        {
            Cache = [];
            Chain = [];
            return;
        }

        // reset all
        ClearCache(0);
    }

    /// <summary>
    /// Deletes all cache entries after `fromDate` (inclusive)
    /// </summary>
    /// <param name="fromTimestamp">From date, inclusive</param>
    /// <exception cref="InvalidOperationException">
    /// `fromDate` not found
    /// </exception>
    public void ClearCache(DateTime fromTimestamp)
    {
        int s = Cache.FindIndex(fromTimestamp);  // start of range

        if (s == -1)
        {
            throw new InvalidOperationException(
                "Cache clear starting target not found.");
        }

        ClearCache(s);
    }

    /// <summary>
    /// Deletes all cache entries after `fromIndex` (inclusive)
    /// </summary>
    /// <param name="fromIndex">From index, inclusive</param>
    internal void ClearCache(int fromIndex) => ClearCache(fromIndex, toIndex: Cache.Count - 1);

    /// <summary>
    /// Deletes cache and chain entries between index range values, inclusively.
    /// It is implemented in inheriting classes due to unique requirements.
    /// </summary>
    /// <param name="fromIndex">First element to delete</param>
    /// <param name="toIndex">Last element to delete</param>
    internal abstract void ClearCache(int fromIndex, int toIndex);

    /// <summary>
    /// Replay from supplier cache start date, inclusive
    /// </summary>
    /// <param name="fromTimestamp">First element to rebuild</param>
    /// <param name="offset">Offset start index</param>
    /// <exception cref="InvalidOperationException"></exception>
    internal abstract void RebuildCache(DateTime fromTimestamp, int offset = 0);

    /// <summary>
    /// Replay from supplier cache index, inclusive
    /// </summary>
    /// <param name="fromIndex">First element to rebuild</param>
    /// <param name="offset">Offset start index</param>
    internal abstract void RebuildCache(int fromIndex, int offset = 0);

    /// <summary>
    /// Overload for non-chainable cachors that do not store chain values.
    /// </summary>
    /// <param name="r"></param>
    /// <returns cref="Act">Action taken</returns>
    internal Act CacheWithAnalysis(TSeries r) => CacheWithAnalysis(r, double.NaN);

    /// <summary>
    /// Analyze and ADD new arrival to cache, after determining best instruction.
    /// </summary>
    /// <param name="r" cref="ISeries">
    ///   Fully formed cacheable time-series object.
    /// </param>
    /// <param name="value">Meaningful chain observable value.  Unused if not a Chainor.</param>
    /// <returns cref="Act">Action taken</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OverflowException"></exception>
    internal Act CacheWithAnalysis(TSeries r, double value)
    {
        /* ANALYZE NEW VALUE (then act)
         *
         * Analyze new indicator value against the cache for proper handling.
         * Note the "value" is optional as only some indicators are chainable
         * and would need to compile the Chain cache.  Only the main Cache is
         * evaluated as both are expected to be synchronized automatically in
         * the following AddPerAction method.  The IsChainor boolean is set
         * to simplify the determination of whether TSeries is a chainable type.
         *
         * Currently, only inbound Quote is accepted as an external chain entry
         * point and is the only type using this method.   -- DS 12/4/2023
         *
         * TODO: consider moving analysis to QuoteProvider, if it's the only user.
         */

        // null TSeries is not expected
        if (r == null)
        {
            throw new ArgumentNullException(
                nameof(r),
                "Unexpected null TSeries in Cache add analyzer.");
        }

        // REPEAT AND OVERFLOW PROTECTION

        if (r.Timestamp == LastArrival.Timestamp)
        {
            // note: we have a better IsEqual() comparison method below,
            // but it is too expensive as an initial quick evaluation.

            OverflowCount++;

            if (OverflowCount > 100)
            {
                string msg = "A repeated stream update exceeded the 100 attempt threshold. "
                           + "Check and remove circular chains or check your stream provider."
                           + "Provider terminated.";

                // note: if provider, catch overflow exception in parent observable,
                // where it will EndTransmission(); and then throw to user.

                throw new OverflowException(msg);
            }

            // aggressive property value comparison
            // TODO: not handling add-back after delete, registers as dup
            if (r.Equals(LastArrival))
            {
                // to prevent propogation
                // of identical cache entry
                return Act.DoNothing;
            }

            // same date with different values continues as an update,
            // but still counts towards overflow threshold
            else
            {
                LastArrival = r;
            }
        }
        else
        {
            OverflowCount = 0;
            LastArrival = r;
        }

        // DETERMINE ACTion INSTRUCTION

        Act act;
        List<TSeries> cache = Cache;
        int length = cache.Count;

        // first
        if (length == 0)
        {
            act = Act.AddNew;
            return ModifyPerAction(act, r, value);
        }

        ISeries last = cache[length - 1];

        // newer
        if (r.Timestamp > last.Timestamp)
        {
            act = Act.AddNew;
        }

        // repeat or late arrival
        else
        {
            // seek duplicate
            int foundIndex = cache
                .FindIndex(x => x.Timestamp == r.Timestamp);

            // replace duplicate
            act = foundIndex == -1 ? Act.AddOld : Act.Update;
        }

        // perform actual update, return final action
        return ModifyPerAction(act, r, value);
    }

    /// <summary>
    /// Analyze and DELETE new arrivals from cache, after determining best instruction.
    /// </summary>
    /// <param name="r" cref="ISeries">
    ///   Fully formed cacheable time-series object.
    /// </param>
    /// <returns cref="Act">Action taken</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OverflowException"></exception>
    internal Act PurgeWithAnalysis(TSeries r)
    {
        // null TSeries is not expected
        if (r == null)
        {
            throw new ArgumentNullException(
                nameof(r),
                "Unexpected null TSeries in Cache purge analyzer.");
        }

        // REPEAT AND OVERFLOW PROTECTION

        if (r.Timestamp == LastArrival.Timestamp)
        {
            // note: we have a better IsEqual() comparison method below,
            // but it is too expensive as an initial quick evaluation.

            OverflowCount++;

            if (OverflowCount > 100)
            {
                string msg = "A repeated stream update exceeded the 100 attempt threshold. "
                           + "Check and remove circular chains or check your stream provider."
                           + "Provider terminated.";

                // note: if provider, catch overflow exception in parent observable,
                // where it will EndTransmission(); and then throw to user.

                throw new OverflowException(msg);
            }

            // note: aggressive property value comparison is often
            // not possible for deletes due to an inability to re-calculate prior values
            // TODO: not handling add-back after delete, registers as dup
        }
        else
        {
            OverflowCount = 0;
            LastArrival = r;
        }

        // determine if record exists
        int foundIndex = Cache
            .FindIndex(x => x.Timestamp == r.Timestamp);

        // not found
        if (foundIndex == -1)
        {
            return Act.DoNothing;
        }

        TSeries t = Cache[foundIndex];

        // delete if full match
        return t.Equals(r)
            ? ModifyPerAction(Act.Delete, t, double.NaN)
            : Act.DoNothing;
    }

    // overload for chainable cachors
    internal Act CacheChainorPerAction(Act act, TSeries r, double value)
        => ModifyPerAction(act, r, value);

    // overload for non-chainable cachors
    internal Act CacheResultPerAction(Act act, TSeries r)
        => ModifyPerAction(act, r, double.NaN);

    // update cache, per instruction act
    private Act ModifyPerAction(Act act, TSeries r, double value)
    {
        /* MODIFY THE CACHE
         *
         * This performs the actual modification to the cache.
         * If the indicator is also a chain provider, also add to the
         * standard observable tuple format.  Timestamp and value parity
         * must be maintained between these two cache and chain repos.
         *
         * Historically, we've had trouble simply reusing the cache
         * as the source for re-building the observer caches due to
         * the use of ChainProvider as the handler, a separate entity.
         * There is potential to remove this redundant Chain cache,
         * however, I could not find a satisfactory and performant
         * alternative to this separation.    -- DS 12/4/2023
         */

        List<TSeries> cache = Cache;
        List<(DateTime Timestamp, double Value)> chain = Chain;

        (DateTime Timestamp, double) t = (r.Timestamp, value);

        // execute action
        switch (act)
        {
            case Act.AddNew:

                cache.Add(r);

                if (isChainor)
                {
                    chain.Add(t);
                }

                break;

            case Act.AddOld:

                // find
                int ao = Cache.FindIndex(x => x.Timestamp > r.Timestamp);

                // insert
                if (ao != -1)
                {
                    cache.Insert(ao, r);

                    if (isChainor)
                    {
                        chain.Insert(ao, t);
                    }
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
                int uo = Cache.FindIndex(r.Timestamp);

                // replace
                if (uo != -1)
                {
                    cache[uo] = r;

                    if (isChainor)
                    {
                        chain[uo] = t;
                    }
                }

                // failure to find should never happen
                else
                {
                    throw new InvalidOperationException(
                        "Cache update target not found.");
                }

                break;

            case Act.Delete:

                // find
                int d = cache.FindIndex(r.Timestamp);

                // delete
                if (d != -1)
                {
                    cache.RemoveAt(d);

                    if (isChainor)
                    {
                        chain.RemoveAt(d);
                    }
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

        return act;
    }
}
