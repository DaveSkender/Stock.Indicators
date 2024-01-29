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
    public static Collection<(DateTime TickDate, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
        => reusable
            .ToTupleResult()
            .ToSortedCollection();

    // CONVERT TO TUPLE with non-nullable NaN value option and no pruning
    internal static List<(DateTime TickDate, double Value)> ToTupleResult<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
    {
        List<TResult> reList = reusable.ToSortedList();
        int length = reList.Count;

        List<(DateTime, double)> results = [];

        for (int i = 0; i < length; i++)
        {
            TResult r = reList[i];
            results.Add(new(r.TickDate, r.Value));
        }

        return results;
    }

    // TODO: are these needed for custom indicators public API?
    internal static (DateTime TickDate, double Value) ToTupleResult<TResult>(
        this TResult result)
        where TResult : IReusableResult
            => new(result.TickDate, result.Value);

    // TODO: are these needed for custom indicators public API?
    internal static (Act, DateTime, double) ToTupleResult<TResult>(
        this TResult result, Act act)
        where TResult : IReusableResult
            => new(act, result.TickDate, result.Value);
}
