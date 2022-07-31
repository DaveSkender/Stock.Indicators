namespace Skender.Stock.Indicators;

// MOTHER of ADAPTIVE MOVING AVERAGES - MAMA (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<MamaResult> GetMama<TQuote>(
        this IEnumerable<TQuote> quotes,
        double fastLimit = 0.5,
        double slowLimit = 0.05)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.HL2)
            .CalcMama(fastLimit, slowLimit);

    // SERIES, from CHAIN
    public static IEnumerable<MamaResult> GetMama(
        this IEnumerable<IReusableResult> results,
        double fastLimit = 0.5,
        double slowLimit = 0.05) => results
            .ToResultTuple()
            .CalcMama(fastLimit, slowLimit)
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<MamaResult> GetMama(
        this IEnumerable<(DateTime, double)> priceTuples,
        double fastLimit = 0.5,
        double slowLimit = 0.05) => priceTuples
            .ToSortedList()
            .CalcMama(fastLimit, slowLimit);
}
