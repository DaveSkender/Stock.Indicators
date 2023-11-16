using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSOLETE IN v2.0.0
public static partial class Indicator
{
#pragma warning disable CA1002 // Do not expose generic lists

    // 2.4.1
    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ToBasicTuple(..)' to 'ToTuple(..)' to fix.", false)]
    public static List<(DateTime, double)> ToBasicTuple<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart)
        where TQuote : IQuote
        => quotes.ToTuple(candlePart);

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ToResultTuple(..)' to 'ToTuple(..)' to fix.", false)]
    public static List<(DateTime Date, double Value)> ToResultTuple(
        this IEnumerable<IReusableResult> basicData)
        => basicData.ToTuple();

    // v2.4.8
    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ToTupleCollection(..)' to 'ToTupleChainable(..)' to fix.", false)]
    public static Collection<(DateTime Date, double Value)> ToTupleCollection(
    this IEnumerable<IReusableResult> reusable)
        => reusable
            .ToTupleChainable();

    [ExcludeFromCodeCoverage]
    [Obsolete("Rename 'ToTupleCollection(NullTo..)' to either 'ToTupleNaN(..)' or 'ToTupleNull(..)' to fix.", false)]
    public static Collection<(DateTime Date, double? Value)> ToTupleCollection(
    this IEnumerable<IReusableResult> reusable, NullTo nullTo)
    {
        List<IReusableResult> reList = reusable.ToSortedList();
        int length = reList.Count;

        Collection<(DateTime Date, double? Value)> results = [];

        for (int i = 0; i < length; i++)
        {
            IReusableResult r = reList[i];
            results.Add(new(r.Date, r.Value.Null2NaN()));
        }

        return results;
    }

    // v2.4.10
    [ExcludeFromCodeCoverage]
    [Obsolete("Change 'GetStarcBands()' to 'GetStarcBands(20)' to fix.", false)]
    public static IEnumerable<StarcBandsResult> GetStarcBands<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.GetStarcBands(20);

#pragma warning restore CA1002 // Do not expose generic lists
}

// v2.4.8 (see above)
public enum NullTo
{
    NaN,
    Null
}
