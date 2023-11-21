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
                x => double.IsNaN(x.Value));

        return resultsList.ToSortedList();
    }

    // CONVERT TO TUPLE (default with pruning)
    /// <include file='./info.xml' path='info/type[@name="TupleChain"]/*' />
    ///
    public static Collection<(DateTime Date, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
    {
        Collection<(DateTime date, double value)> tuples = [];
        List<TResult> reList = reusable.ToSortedList();

        // find first non-nulled
        for (int i = 0; i < reList.Count; i++)
        {
            IReusableResult? r = reList[i];
            tuples.Add(new(r.Date, r.Value));
        }

        return tuples;
    }

    // CONVERT TO TUPLE with non-nullable NaN value option and no pruning
    internal static List<(DateTime Date, double Value)> ToTupleResult<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
    {
        List<TResult> reList = reusable.ToSortedList();
        int length = reList.Count;

        List<(DateTime, double)> results = [];

        for (int i = 0; i < length; i++)
        {
            TResult r = reList[i];
            results.Add(new(r.Date, r.Value));
        }

        return results;
    }

    // TODO: are these needed for custom indicators public API?
    internal static (DateTime Date, double Value) ToTupleResult<TResult>(
        this TResult result)
        where TResult : IReusableResult
            => new(result.Date, result.Value);
}
