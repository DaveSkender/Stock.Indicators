namespace Skender.Stock.Indicators;

/// <summary>
/// Keltner Channels for a series of quotes indicator.
/// </summary>
public static partial class Keltner
{
    /// <summary>
    /// Converts a list of quotes to Keltner Channel results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="emaPeriods">The number of periods for the EMA.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="atrPeriods">The number of periods for the ATR.</param>
    /// <returns>A list of Keltner Channel results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    public static IReadOnlyList<KeltnerResult> ToKeltner(
        this IReadOnlyList<IQuote> quotes,
        int emaPeriods = 20,
        double multiplier = 2,
        int atrPeriods = 10)
        => quotes
            .ToQuoteDList()
            .CalcKeltner(emaPeriods, multiplier, atrPeriods);

    /// <summary>
    /// Calculates the Keltner Channel for a list of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="emaPeriods">The number of periods for the EMA.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="atrPeriods">The number of periods for the ATR.</param>
    /// <returns>A list of Keltner Channel results.</returns>
    private static List<KeltnerResult> CalcKeltner(
        this List<QuoteD> quotes,
        int emaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        Validate(emaPeriods, multiplier, atrPeriods);

        // initialize
        int length = quotes.Count;
        List<KeltnerResult> results = new(length);

        IReadOnlyList<EmaResult> emaResults
            = quotes.ToEma(emaPeriods);

        List<AtrResult> atrResults
            = quotes.CalcAtr(atrPeriods);

        int lookbackPeriods = Math.Max(emaPeriods, atrPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];

            if (i >= lookbackPeriods - 1)
            {
                EmaResult ema = emaResults[i];
                AtrResult atr = atrResults[i];
                double? atrSpan = atr.Atr * multiplier;

                results.Add(new KeltnerResult(
                    Timestamp: q.Timestamp,
                    UpperBand: ema.Ema + atrSpan,
                    Centerline: ema.Ema,
                    LowerBand: ema.Ema - atrSpan,
                    Width: ema.Ema == 0 ? null : 2 * atrSpan / ema.Ema) {
                    Atr = atr.Atr
                });
            }
            else
            {
                results.Add(new(q.Timestamp));
            }
        }

        return results;
    }
}
