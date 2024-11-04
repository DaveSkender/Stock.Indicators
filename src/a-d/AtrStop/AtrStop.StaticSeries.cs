namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the ATR Trailing Stop.
/// </summary>
public static partial class AtrStop
{
    /// <summary>
    /// Calculates the ATR Trailing Stop from a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 21.</param>
    /// <param name="multiplier">The multiplier for the ATR. Default is 3.</param>
    /// <param name="endType">The type of price to use for the calculation. Default is <see cref="EndType.Close"/>.</param>
    /// <returns>A list of ATR Trailing Stop results.</returns>
    public static IReadOnlyList<AtrStopResult> ToAtrStop<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 21,
        double multiplier = 3,
        EndType endType = EndType.Close)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcAtrStop(lookbackPeriods, multiplier, endType);

    /// <summary>
    /// Calculates the ATR Trailing Stop for the given source data.
    /// </summary>
    /// <param name="source">The source data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    /// <returns>A list of ATR Trailing Stop results.</returns>
    private static List<AtrStopResult> CalcAtrStop(
        this IReadOnlyList<QuoteD> source,
        int lookbackPeriods,
        double multiplier,
        EndType endType)
    {
        // check parameter arguments
        Validate(lookbackPeriods, multiplier);

        // initialize
        int length = source.Count;
        List<AtrStopResult> results = new(length);
        List<AtrResult> atrResults = source.CalcAtr(lookbackPeriods);

        // prevailing direction and bands
        bool isBullish = true;
        double upperBand = double.MaxValue;
        double lowerBand = double.MinValue;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            // handle warmup periods
            if (i < lookbackPeriods)
            {
                results.Add(new(Timestamp: source[i].Timestamp));
                continue;
            }

            QuoteD q = source[i];
            QuoteD p = source[i - 1];

            // initialize direction on first evaluation
            if (i == lookbackPeriods)
            {
                isBullish = q.Close >= p.Close;
            }

            // evaluate bands
            double upperEval;
            double lowerEval;
            double atr = atrResults[i].Atr ?? double.NaN;

            // potential bands for CLOSE
            if (endType == EndType.Close)
            {
                upperEval = q.Close + (multiplier * atr);
                lowerEval = q.Close - (multiplier * atr);
            }

            // potential bands for HIGH/LOW
            else
            {
                upperEval = q.High + (multiplier * atr);
                lowerEval = q.Low - (multiplier * atr);
            }

            // new upper band: can only go down, or reverse
            if (upperEval < upperBand || p.Close > upperBand)
            {
                upperBand = upperEval;
            }

            // new lower band: can only go up, or reverse
            if (lowerEval > lowerBand || p.Close < lowerBand)
            {
                lowerBand = lowerEval;
            }

            // trailing stop: based on direction

            AtrStopResult r;

            // the upper band (short / buy-to-stop)
            if (q.Close <= (isBullish ? lowerBand : upperBand))
            {
                isBullish = false;

                r = new(
                    Timestamp: q.Timestamp,
                    AtrStop: upperBand,
                    BuyStop: upperBand,
                    SellStop: null,
                    Atr: atr);
            }

            // the lower band (long / sell-to-stop)
            else
            {
                isBullish = true;

                r = new(
                    Timestamp: q.Timestamp,
                    AtrStop: lowerBand,
                    BuyStop: null,
                    SellStop: lowerBand,
                    Atr: atr);
            }

            results.Add(r);
        }

        return results;
    }
}
