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
            .ToTuple(CandlePart.HL2)
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
            .ToTupleResult()
            .CalcAlligator(
                jawPeriods,
                jawOffset,
                teethPeriods,
                teethOffset,
                lipsPeriods,
                lipsOffset);

    // SERIES, from TUPLE
    // TODO: is this variant still needed, or just an extra option (all indicators)
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

    // OBSERVER, from Quote Provider
    public static Alligator AttachAlligator<TQuote>(
        this QuoteProvider<TQuote> quoteProvider,
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
        where TQuote : IQuote, new()
    {
        Use<TQuote> chainProvider = quoteProvider
            .Use(CandlePart.HL2);

        return new(
            chainProvider,
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);
    }

    // OBSERVER, from Chain Provider
    public static Alligator AttachAlligator(
        this ChainProvider chainProvider,
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
        => new(
            chainProvider,
            jawPeriods,
            jawOffset,
            teethPeriods,
            teethOffset,
            lipsPeriods,
            lipsOffset);
}
