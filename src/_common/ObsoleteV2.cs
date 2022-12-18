using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSOLETE IN v2.0.0
public static partial class Indicator
{
    // 2.4.1
    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ToBasicTuple(..)' to 'ToTuple(..)' to fix.", false)]
    public static Collection<(DateTime, double)> ToBasicTuple<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes.ToTuple(candlePart);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ToResultTuple(..)' to 'ToTuple(..)' to fix.", false)]
    public static Collection<(DateTime Date, double Value)> ToResultTuple(
        this IEnumerable<IReusableResult> basicData)
        => basicData.ToTuple();

    // 2.5.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ToSortedList(..)' to 'ToSortedCollection(..)' to fix.", false)]
    public static List<TResult> ToSortedList<TResult>(
        this IEnumerable<TResult> results)
        where TResult : IResult => results
            .OrderBy(x => x.Date)
            .ToList();
}