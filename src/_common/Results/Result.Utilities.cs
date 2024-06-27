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
        where TResult : IReusable
    {
        List<TResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => double.IsNaN(x.Value));

        return resultsList.ToSortedList();
    }
}
