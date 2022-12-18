using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // RENKO CHART - ATR (SERIES)
    internal static Collection<RenkoResult> CalcRenkoAtr<TQuote>(
        this Collection<TQuote> quotesList,
        int atrPeriods,
        EndType endType = EndType.Close)
        where TQuote : IQuote
    {
        // initialize
        Collection<AtrResult> atrResults = quotesList
            .ToQuoteD()
            .CalcAtr(atrPeriods);

        double? atr = atrResults.LastOrDefault()?.Atr;
        decimal brickSize = (atr == null) ? 0 : (decimal)atr;

        return brickSize is 0 ?
            new Collection<RenkoResult>()
          : quotesList.CalcRenko(brickSize, endType);
    }
}
