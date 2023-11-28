namespace Skender.Stock.Indicators;

public static class CacheUtility
{
    // METHODS

    // TODO: check cases of direct add, when not observing (e.g. external).  Possibly overanalyzing.
    /*
       Cache analysis scenario considerations:

       (1) Managed provider: when an INDICATOR has a providing observable, that provider will send along the action taken.
           In this case, the observer cache should be aligned and should take the same action without further analysis.
       (2) External provider: when the user provides the quotes directly to an INDICATOR, the analysis must occur before
           updating its own results cache.
       (3) Top-level provider: when the cache represents Quotes (or potentially a tuple equivalent), an original analysis
           must occur when updating its own cache, but also to pass along to observers.

       These two methods are used to cover both scenarios.  The first takes the Act from source and simply
       executes.  The second will perform the analysis before executing the cache update.
    */

    // update cache, after determining instruction act
    // this is only used when there is no internal observable source
    // and an original analysis needs to be done

    internal static Act CacheWithAnalysis<TSeries>(
        this IProvider<TSeries> provider, TSeries r)
        where TSeries : ISeries, new()
    {
        Act act;
        List<TSeries> cache = provider.Cache;
        int length = cache.Count;

        // nulls values are not expected
        if (r == null)
        {
            throw new ArgumentNullException(nameof(r), "Null equality comparison.");
        }

        // check for overflow and repeat condition
        if (r.Date == provider.LastArrival.Date)  // TODO: remove "if (r.IsEqual(provider.LastArrival))" utility if too expensive
        {
            provider.OverflowCount++;

            if (provider.OverflowCount > 100)
            {
                // TODO: throw overflow exception in parent observable,
                // where it will EndTransmission(); and only then throw OverflowException

                string msg = "A repeated stream update exceeded the 100 attempt threshold. "
                           + "Check and remove circular chains or check your stream provider."
                           + "Provider terminated.";

                throw new OverflowException(msg);
            }

            return Act.DoNothing;
        }
        else
        {
            provider.OverflowCount = 0;
            provider.LastArrival = r;
        }


        // first
        if (length == 0)
        {
            act = Act.AddNew;
            return provider.CacheWithAction(act, r);
        }

        ISeries last = cache[length - 1];

        // newer
        if (r.Date > last.Date)
        {
            act = Act.AddNew;
        }

        // current
        else if (r.Date == last.Date)
        {
            act = Act.UpdateLast;
        }

        // late arrival
        else
        {
            // seek duplicate
            int foundIndex = cache
                .FindIndex(x => x.Date == r.Date);

            // replace duplicate
            act = foundIndex == -1 ? Act.AddOld : Act.UpdateOld;
        }

        // perform actual update, return final action
        return provider.CacheWithAction(act, r);
    }


    // update cache, per instruction act
    // TODO: why are we adding superfluous checks? Can't we trust that we've coded the obervable analysis situations correctly?

    internal static Act CacheWithAction<TSeries>(
        this IProvider<TSeries> provider, Act act, TSeries r)
        where TSeries : ISeries, new()
    {
        List<TSeries> cache = provider.Cache;

        // execute action
        switch (act)
        {
            case Act.AddNew:

                cache.Add(r);
                break;

            case Act.AddOld:

                cache.Add(r);

                // re-sort cache
                provider.Cache = [.. cache.OrderBy(x => x.Date)];

                break;

            case Act.UpdateLast:

                int length = cache.Count;

                // update confirmed last entry
                if (r.Date == cache[length - 1].Date)
                {
                    cache[length - 1] = r;
                }

                // failover to UpdateOld
                else
                {
                    act = Act.UpdateOld;
                    return provider.CacheWithAction(act, r);
                }

                break;

            case Act.UpdateOld:

                // find
                int i = cache.FindIndex(r.Date);

                // replace
                if (i != -1)
                {
                    cache[i] = r;
                }

                // failover to AddOld
                else
                {
                    act = Act.AddOld;
                    return provider.CacheWithAction(act, r);
                }

                break;

            case Act.Delete:

                // find
                int d = cache.FindIndex(r.Date);

                // delete
                if (d != -1)
                {
                    cache.RemoveAt(d);
                }

                // nothing to delete
                else
                {
                    act = Act.DoNothing;
                }

                break;

            case Act.DoNothing:

                break;

            default:
                // should never get here
                throw new InvalidOperationException();
        }

        return act;
    }


    // delete cache, gracefully
    public static void ResetCache<TResult>(
        this IProvider<TResult> provider,
        List<IObserver<(Act, DateTime, double)>> observers)
        where TResult : IReusableResult, new()
    {
        int length = provider.Cache.Count;

        if (length > 0)
        {
            // delete and deliver instruction,
            // in reverse order to prevent recompositions
            for (int i = length - 1; i > 0; i--)
            {
                TResult r = provider.Cache[i];
                Act act = provider.CacheWithAction(Act.Delete, r);
                observers.Notify((act, r.Date, r.Value));
            }
        }
    }

    internal static void Notify(
        this List<IObserver<(Act, DateTime, double)>> observers,
        (Act, DateTime, double) chainMessage)
    {
        List<IObserver<(Act, DateTime, double)>> obsList = [.. observers];

        for (int i = 0; i < obsList.Count; i++)
        {
            IObserver<(Act, DateTime, double)> obs = obsList[i];
            obs.OnNext(chainMessage);
        }
    }
}
