namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Keltner Channels for a series of quotes.
/// </summary>
public static partial class Keltner
{
    /// <summary>
    /// Converts a list of quotes to Keltner Channel results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The list of quotes to transform.</param>
    /// <param name="emaPeriods">The number of periods for the EMA. Default is 20.</param>
    /// <param name="multiplier">The multiplier for the ATR. Default is 2.</param>
    /// <param name="atrPeriods">The number of periods for the ATR. Default is 10.</param>
    /// <returns>A list of Keltner Channel results.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    [Series("KELTNER", "Keltner Channels", Category.PriceChannel, ChartType.Overlay)]
    public static IReadOnlyList<KeltnerResult> ToKeltner<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [ParamNum<int>("EMA Periods", 2, 250, 20)]
        int emaPeriods = 20,
        [ParamNum<double>("Multiplier", 0.01, 10, 2)]
        double multiplier = 2,
        [ParamNum<int>("ATR Periods", 2, 250, 10)]
        int atrPeriods = 10)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcKeltner(emaPeriods, multiplier, atrPeriods);

    /// <summary>
    /// Calculates the Keltner Channel for a list of quotes.
    /// </summary>
    /// <param name="source">The list of quotes to process.</param>
    /// <param name="emaPeriods">The number of periods for the EMA.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="atrPeriods">The number of periods for the ATR.</param>
    /// <returns>A list of Keltner Channel results.</returns>
    private static List<KeltnerResult> CalcKeltner(
        this List<QuoteD> source,
        int emaPeriods,
        double multiplier,
        int atrPeriods)
    {
        // check parameter arguments
        Validate(emaPeriods, multiplier, atrPeriods);

        // initialize
        int length = source.Count;
        List<KeltnerResult> results = new(length);

        IReadOnlyList<EmaResult> emaResults
            = source.ToEma(emaPeriods);

        List<AtrResult> atrResults
            = source.CalcAtr(atrPeriods);

        int lookbackPeriods = Math.Max(emaPeriods, atrPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            if (i >= lookbackPeriods - 1)
            {
                EmaResult ema = emaResults[i];
                AtrResult atr = atrResults[i];
                double? atrSpan = atr.Atr * multiplier;

                results.Add(new KeltnerResult(
                    Timestamp: q.Timestamp,
                    UpperBand: ema.Ema + atrSpan,
                    LowerBand: ema.Ema - atrSpan,
                    Centerline: ema.Ema,
                    Width: ema.Ema == 0 ? null : 2 * atrSpan / ema.Ema));
            }
            else
            {
                results.Add(new(q.Timestamp));
            }
        }

        return results;
    }
}
