namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // RENKO CHART (ATR VARIANT)
    /// <include file='./info.xml' path='indicator/type[@name="atr"]/*' />
    ///
    public static IEnumerable<RenkoResult> GetRenkoAtr<TQuote>(
        this IEnumerable<TQuote> quotes,
        int atrPeriods,
        EndType endType = EndType.Close)
        where TQuote : IQuote
    {
        // initialize
        IEnumerable<AtrResult> atrResults = quotes.GetAtr(atrPeriods);

        decimal? atr = atrResults.LastOrDefault()?.Atr;
        decimal brickSize = (atr == null) ? 0 : (decimal)atr;

        return brickSize is 0 ?
            new List<RenkoResult>()
          : quotes.GetRenko(brickSize, endType);
    }
}
