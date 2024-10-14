namespace Skender.Stock.Indicators;

// RENKO CHART - ATR (API)
public static partial class RenkoAtr
{
    public static IReadOnlyList<RenkoResult> GetRenkoAtr<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int atrPeriods,
        EndType endType = EndType.Close)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcRenkoAtr(atrPeriods, endType);
}
