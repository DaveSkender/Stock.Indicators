namespace Skender.Stock.Indicators;

// HELPER FUNCTIONS

public static partial class Indicator
{
    // FIND by DATE
    /// <include file='./info.xml' path='info/type[@name="Find"]/*' />
    ///
    public static TResult? Find<TResult>(
        this IEnumerable<TResult> results,
        DateTime lookupDate)
        where TResult : IResult => results.FirstOrDefault(x => x.Date == lookupDate);

    // REMOVE SPECIFIC PERIODS extension
    /// <include file='./info.xml' path='info/type[@name="PruneT"]/*' />
    ///
    public static IEnumerable<TResult> RemoveWarmupPeriods<TResult>(
        this IEnumerable<TResult> results,
        int removePeriods)
        where TResult : IResult
        => removePeriods < 0
            ? throw new ArgumentOutOfRangeException(nameof(removePeriods), removePeriods,
                "If specified, the Remove Periods value must be greater than or equal to 0.")
            : results.Remove(removePeriods);

    // CONDENSE (REMOVE null and NaN results)
    /// <include file='./info.xml' path='info/type[@name="CondenseT"]/*' />
    ///
    public static IEnumerable<TResult> Condense<TResult>(
        this IEnumerable<TResult> results)
        where TResult : IReusableResult
    {
        List<TResult> resultsList = results
            .ToList();

        _ = resultsList
            .RemoveAll(match:
                x => x.Value is null || x.Value is double and double.NaN);

        return resultsList.ToSortedList();
    }

    // SYNC INDEX - RESIZE TO MATCH OTHER
    public static IEnumerable<TResultR> SyncIndex<TResultR, TResultM>(
        this IEnumerable<TResultR> results,
        IEnumerable<TResultM> resultsToMatch,
        SyncType syncType = SyncType.FullMatch)
        where TResultR : IResult
        where TResultM : IResult
    {
        // initialize
        List<TResultR> resultsList = results.ToSortedList();
        List<TResultM> matchList = resultsToMatch.ToSortedList();

        if (matchList.Count == 0 || resultsList.Count == 0)
        {
            return new List<TResultR>();
        }

        bool prepend = false;
        bool append = false;
        bool remove = false;

        switch (syncType)
        {
            case SyncType.Prepend:
                prepend = true;
                break;

            case SyncType.AppendOnly:
                prepend = append = true;
                break;

            case SyncType.RemoveOnly:
                remove = true;
                break;

            case SyncType.FullMatch:
                prepend = append = remove = true;
                break;
        }

        Type type = resultsList[0].GetType();

        // add plugs for missing values
        if (prepend || append)
        {
            List<TResultR> toAppend = new();

            for (int i = 0; i < matchList.Count; i++)
            {
                TResultM? m = matchList[i];
                TResultR? r = resultsList.Find(m.Date);

                if (r is null)
                {
                    TResultR? n = (TResultR?)Activator.CreateInstance(type, m.Date);
                    if (n != null)
                    {
                        toAppend.Add(n);
                    }
                }
                else if (!append)
                {
                    break;
                }
            }

            resultsList.AddRange(toAppend);
        }

        // remove unmatched results
        if (remove)
        {
            List<TResultR> toRemove = new();

            for (int i = 0; i < resultsList.Count; i++)
            {
                TResultR? r = resultsList[i];
                TResultM? m = matchList.Find(r.Date);

                if (m is null)
                {
                    toRemove.Add(r);
                }
            }

            resultsList.RemoveAll(x => toRemove.Contains(x));
        }

        return resultsList.ToSortedList();
    }

    // CONVERT TO TUPLE
    internal static List<(DateTime Date, double Value)> ToResultTuple(
        this IEnumerable<IReusableResult> basicData)
    {
        List<(DateTime Date, double Value)> prices = new();
        List<IReusableResult>? bdList = basicData.ToList();

        // find first non-nulled
        int first = bdList.FindIndex(x => x.Value != null);

        for (int i = first; i < bdList.Count; i++)
        {
            IReusableResult? q = bdList[i];
            prices.Add(new(q.Date, NullMath.Null2NaN(q.Value)));
        }

        return prices.OrderBy(x => x.Date).ToList();
    }

    // RETURN SORTED LIST of RESULTS
    internal static List<TResult> ToSortedList<TResult>(
        this IEnumerable<TResult> results)
        where TResult : IResult => results
            .OrderBy(x => x.Date)
            .ToList();

    // REMOVE RESULTS
    private static List<TResult> Remove<TResult>(
        this IEnumerable<TResult> results,
        int removePeriods)
        where TResult : IResult
    {
        List<TResult> resultsList = results.ToList();

        if (resultsList.Count <= removePeriods)
        {
            return new List<TResult>();
        }
        else
        {
            if (removePeriods > 0)
            {
                for (int i = 0; i < removePeriods; i++)
                {
                    resultsList.RemoveAt(0);
                }
            }

            return resultsList;
        }
    }
}
