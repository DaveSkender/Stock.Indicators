namespace Skender.Stock.Indicators;

// RENKO CHART - ATR (SERIES)

public static partial class Renko
{
    public static IReadOnlyList<RenkoResult> GetRenkoAtr<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int atrPeriods,
        EndType endType = EndType.Close)
        where TQuote : IQuote
    {
        // initialize
        List<AtrResult> atrResults = quotes
            .ToQuoteDList()
            .CalcAtr(atrPeriods);

        AtrResult? last = atrResults.LastOrDefault();
        decimal brickSize = (decimal?)last?.Atr ?? 0;

        return brickSize == 0
          ? []
          : quotes.ToRenko(brickSize, endType);
    }
}
