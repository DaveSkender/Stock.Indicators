namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<GatorResult> GetGator<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote => quotes
            .ToBasicTuple(CandlePart.HL2)
            .GetAlligator()
            .ToList()
            .CalcGator();

    // SERIES, from [custom] Alligator
    public static IEnumerable<GatorResult> GetGator(
        this IEnumerable<AlligatorResult> alligator) => alligator
            .ToList()
            .CalcGator();

    // SERIES, from CHAIN
    public static IEnumerable<GatorResult> GetGator(
        this IEnumerable<IReusableResult> results) => results
            .ToResultTuple()
            .GetAlligator()
            .ToList()
            .CalcGator()
            .SyncIndex(results, SyncType.Prepend);

    // SERIES, from TUPLE
    public static IEnumerable<GatorResult> GetGator(
        this IEnumerable<(DateTime, double)> priceTuples) => priceTuples
            .ToSortedList()
            .GetAlligator()
            .ToList()
            .CalcGator();
}
