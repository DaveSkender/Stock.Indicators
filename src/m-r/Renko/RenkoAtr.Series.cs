namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // RENKO CHART - ATR (SERIES)
    internal static List<RenkoResult> CalcRenkoAtr<TQuote>(
        this List<TQuote> quotesList,
        int atrPeriods,
        EndType endType = EndType.Close)
        where TQuote : IQuote
    {
        // initialize
        List<AtrResult> atrResults = quotesList
            .ToQuoteD()
            .CalcAtr(atrPeriods);

        double? atr = atrResults.LastOrDefault()?.Atr;
        decimal brickSize = (atr == null) ? 0 : (decimal)atr;

        return brickSize is 0 ?
            new List<RenkoResult>()
          : quotesList.CalcRenko(brickSize, endType);
    }
}
