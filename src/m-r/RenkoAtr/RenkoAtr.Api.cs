namespace Skender.Stock.Indicators;

// RENKO CHART - ATR (API)
public static partial class RenkoAtr
{
    public static IEnumerable<RenkoResult> GetRenkoAtr<TQuote>(
        this IEnumerable<TQuote> quotes,
        int atrPeriods,
        EndType endType = EndType.Close)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcRenkoAtr(atrPeriods, endType);
}
