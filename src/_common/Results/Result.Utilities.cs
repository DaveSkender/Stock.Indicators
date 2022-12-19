using System.Collections.ObjectModel;

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

        resultsList
            .RemoveAll(match:
                x => x.Value is null or (double and double.NaN));

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

    // CONVERT TO TUPLE (default with pruning)
    public static Collection<(DateTime Date, double Value)> ToTupleCollection(
        this IEnumerable<IReusableResult> reusable)
        => reusable
            .ToTuple()
            .ToCollection();

    internal static List<(DateTime Date, double Value)> ToTuple(
        this IEnumerable<IReusableResult> reusable)
    {
        List<(DateTime date, double value)> prices = new();
        List<IReusableResult> reList = reusable.ToList();

        // find first non-nulled
        int first = reList.FindIndex(x => x.Value != null);

        for (int i = first; i < reList.Count; i++)
        {
            IReusableResult? r = reList[i];
            prices.Add(new(r.Date, r.Value.Null2NaN()));
        }

        return prices.OrderBy(x => x.date).ToList();
    }

    // CONVERT TO TUPLE with nullable value option and no pruning
    internal static List<(DateTime Date, double? Value)> ToTuple(
        this IEnumerable<IReusableResult> reusable,
        NullTo nullTo)
    {
        List<IReusableResult> reList = reusable.ToList();
        int length = reList.Count;
        List<(DateTime date, double? value)> prices = new(length);

        for (int i = 0; i < length; i++)
        {
            IReusableResult r = reList[i];
            prices.Add(new(r.Date, (nullTo == NullTo.NaN) ? r.Value.Null2NaN() : r.Value));
        }

        return prices.OrderBy(x => x.date).ToList();
    }

    // RETURN SORTED LIST of RESULTS
    public static Collection<TResult> ToSortedCollection<TResult>(
        this IEnumerable<TResult> results)
        where TResult : IResult
        => results
            .ToSortedList()
            .ToCollection();

    internal static List<TResult> ToSortedList<TResult>(
        this IEnumerable<TResult> results)
        where TResult : IResult
        => results
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
