namespace Skender.Stock.Indicators;

// base result or series cache
public abstract class SeriesCache<TSeries> : ChainProvider
    where TSeries : ISeries, new()
{
    // fields
    private readonly bool isChainor;

    // constructor
    internal SeriesCache()
    {
        Cache = [];
        LastArrival = new();
        OverflowCount = 0;

        Type? reuseType = typeof(TSeries)
            .GetInterface("IReusableResult");

        isChainor = reuseType != null && reuseType.Name == "IReusableResult";
    }

    // PROPERTIES

    public IEnumerable<TSeries> Results => Cache;

    internal List<TSeries> Cache { get; set; }

    internal TSeries LastArrival { get; set; }

    internal int OverflowCount { get; set; }

    // METHODS

    // add a nicely formatted label to end indicatores, e.g. EMA(10)
    public abstract override string ToString();

    // Overload for non-chainable cachors that do not store chain values.
    internal Act CacheWithAnalysis(TSeries r) => CacheWithAnalysis(r, double.NaN);

    /// <summary>
    /// Analyze and cache new arrival, after determining best instruction.
    /// </summary>
    /// <param name="r">Fully formed cache IReusableResult object.</param>
    /// <param name="value">Meaningful chain observable value.  Unused if not a Chainor.</param>
    /// <returns></returns>
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
                "Unexpected null TSeries in Cache analyzer.");
        }

        // REPEAT AND OVERFLOW PROTECTION

        if (r.Date == LastArrival.Date)
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
            if (r.IsEqual(LastArrival))
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
        if (r.Date > last.Date)
        {
            act = Act.AddNew;
        }

        // repeat or late arrival
        else
        {
            // seek duplicate
            int foundIndex = cache
                .FindIndex(x => x.Date == r.Date);

            // replace duplicate
            act = foundIndex == -1 ? Act.AddOld : Act.UpdateOld;
        }

        // perform actual update, return final action
        return ModifyPerAction(act, r, value);
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
         * standard observable tuple format.  Date and value parity
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
        List<(DateTime Date, double Value)> chain = Chain;

        (DateTime Date, double) t = (r.Date, value);

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
                int ao = Cache.FindIndex(x => x.Date > r.Date);

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
                    throw new InvalidOperationException("Cache insert target not found.");
                }

                break;

            case Act.UpdateOld:

                // find
                int uo = Cache.FindIndex(r.Date);

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
                    throw new InvalidOperationException("Cache update target not found.");
                }

                break;

            case Act.Delete:

                // find
                int d = cache.FindIndex(r.Date);

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
                    throw new InvalidOperationException("Cache delete target not found.");
                }

                break;

            case Act.DoNothing:

                break;

            // should never get here
            default:

                throw new InvalidOperationException("Undefined cache action.");
        }

        return act;
    }
};