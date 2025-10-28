namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating Renko chart series using ATR (Average True Range).
/// </summary>
public static partial class RenkoAtr
{
    /// <summary>
    /// Converts a list of quotes to a list of Renko chart results using ATR for brick size.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="atrPeriods">The number of periods for calculating ATR.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <returns>A list of Renko chart results.</returns>
    public static IReadOnlyList<RenkoResult> ToRenkoAtr(
        this IReadOnlyList<IQuote> quotes,
        int atrPeriods = 14,
        EndType endType = EndType.Close)
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
