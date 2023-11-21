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
    // TODO: this can be obsolete, see related IReusableResult task
    public static Collection<(DateTime Date, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
        => reusable
            .ToTuplePruned()
            .ToCollection();

    // TODO: this can be osolete, see related IReusableResult task
    internal static List<(DateTime Date, double Value)> ToTuplePruned<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
    {
        List<(DateTime date, double value)> prices = [];
        List<TResult> reList = reusable.ToList();

        // find first non-nulled
        int first = reList.FindIndex(x => !double.IsNaN(x.Value));

        for (int i = first; i < reList.Count; i++)
        {
            IReusableResult? r = reList[i];
            prices.Add(new(r.Date, r.Value));
        }

        return prices.OrderBy(x => x.date).ToList();
    }

    // CONVERT TO TUPLE with non-nullable NaN value option and no pruning
    /// <include file='./info.xml' path='info/type[@name="TupleResult"]/*' />
    ///
    // TODO: rename to ToResultTuple, simply.
    public static Collection<(DateTime Date, double Value)> ToResultTuple<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
    {
        List<TResult> reList = reusable.ToSortedList();
        int length = reList.Count;

        Collection<(DateTime Date, double Value)> results = [];

        for (int i = 0; i < length; i++)
        {
            TResult r = reList[i];
            results.Add(new(r.Date, r.Value));
        }

        return results;
    }

    public static (DateTime Date, double Value) ToResultTuple<TResult>(
        this TResult result)
        where TResult : IReusableResult
            => new(result.Date, result.Value);
}
