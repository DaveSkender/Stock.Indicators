namespace Skender.Stock.Indicators;

// PRICE RELATIVE STRENGTH (API)
public static partial class Indicator
{
    // SERIES, from CHAINS (both inputs reusable)
    public static IReadOnlyList<PrsResult> ToPrs<T>(
        this IEnumerable<T> quotesEval,
        IEnumerable<T> quotesBase,
        int? lookbackPeriods = null)
        where T : IReusable
        => CalcPrs(
            quotesEval.ToSortedList(),
            quotesBase.ToSortedList(),
            lookbackPeriods);
}
