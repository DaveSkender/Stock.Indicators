using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// RESULTS UTILITIES

public static partial class ResultUtility
{
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

    // CONVERT TO TUPLE (default with pruning)
    /// <include file='./info.xml' path='info/type[@name="TupleChain"]/*' />
    ///
    public static Collection<(DateTime Date, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
        => reusable
            .ToTuple()
            .ToCollection();

    internal static List<(DateTime Date, double Value)> ToTuple<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
    {
        List<(DateTime date, double value)> prices = new();
        List<TResult> reList = reusable.ToList();

        // find first non-nulled
        int first = reList.FindIndex(x => x.Value != null);

        for (int i = first; i < reList.Count; i++)
        {
            IReusableResult? r = reList[i];
            prices.Add(new(r.Date, r.Value.Null2NaN()));
        }

        return prices.OrderBy(x => x.date).ToList();
    }

    // CONVERT TO TUPLE with non-nullable NaN value option and no pruning
    /// <include file='./info.xml' path='info/type[@name="TupleNaN"]/*' />
    ///
    public static Collection<(DateTime Date, double Value)> ToTupleNaN<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
    {
        List<TResult> reList = reusable.ToSortedList();
        int length = reList.Count;

        Collection<(DateTime Date, double Value)> results = new();

        for (int i = 0; i < length; i++)
        {
            IReusableResult r = reList[i];
            results.Add(new(r.Date, r.Value.Null2NaN()));
        }

        return results;
    }
}
