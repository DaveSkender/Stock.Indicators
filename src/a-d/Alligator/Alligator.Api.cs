namespace Skender.Stock.Indicators;

// WILLIAMS ALLIGATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<AlligatorResult> GetAlligator<TQuote>(
        this IEnumerable<TQuote> quotes,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.HL2)
            .CalcAlligator(
                jawPeriods,
                jawOffset,
                teethPeriods,
                teethOffset,
                lipsPeriods,
                lipsOffset);

    // SERIES, from CHAIN
    public static IEnumerable<AlligatorResult> GetAlligator(
        this IEnumerable<IReusableResult> results,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3) => results
            .ToResultTuple()
            .CalcAlligator(
                jawPeriods,
                jawOffset,
                teethPeriods,
                teethOffset,
                lipsPeriods,
                lipsOffset)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<AlligatorResult> GetAlligator(
        this IEnumerable<(DateTime, double)> priceTuples,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3) => priceTuples
            .ToSortedList()
            .CalcAlligator(
                jawPeriods,
                jawOffset,
                teethPeriods,
                teethOffset,
                lipsPeriods,
                lipsOffset);
}
