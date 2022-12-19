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

#pragma warning restore CA1002 // Do not expose generic lists
}