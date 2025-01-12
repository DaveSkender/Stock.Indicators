/// <summary>
/// SuperTrend indicator calculation.
/// </summary>
public static partial class SuperTrend
{
    /// <summary>
    /// Converts a list of quotes to a list of SuperTrend results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <returns>A list of SuperTrend results.</returns>
    public static IReadOnlyList<SuperTrendResult> ToSuperTrend<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 10,
        double multiplier = 3)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcSuperTrend(lookbackPeriods, multiplier);

    /// <summary>
    /// Calculates the SuperTrend indicator.
    /// </summary>
    /// <param name="source">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <returns>A list of SuperTrend results.</returns>
    private static List<SuperTrendResult> CalcSuperTrend(
        this List<QuoteD> source,
        int lookbackPeriods,
        double multiplier)
    {
        // check parameter arguments
        Validate(lookbackPeriods, multiplier);

        // initialize
        int length = source.Count;
        List<SuperTrendResult> results = new(length);
        List<AtrResult> atrResults = source.CalcAtr(lookbackPeriods);

        bool isBullish = true;
        double? upperBand = null;
        double? lowerBand = null;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            double? superTrend;
            double? upperOnly;
            double? lowerOnly;

            if (i >= lookbackPeriods)
            {
                double? mid = (q.High + q.Low) / 2;
                double? atr = atrResults[i].Atr;
                double? prevClose = source[i - 1].Close;

                // potential bands
                double? upperEval = mid + (multiplier * atr);
                double? lowerEval = mid - (multiplier * atr);

                // initial values
                if (i == lookbackPeriods)
                {
                    isBullish = q.Close >= mid;

                    upperBand = upperEval;
                    lowerBand = lowerEval;
                }

                // new upper band
                if (upperEval < upperBand || prevClose > upperBand)
                {
                    upperBand = upperEval;
                }

                // new lower band
                if (lowerEval > lowerBand || prevClose < lowerBand)
                {
                    lowerBand = lowerEval;
                }

                // supertrend
                if (q.Close <= (isBullish ? lowerBand : upperBand))
                {
                    superTrend = upperBand;
                    upperOnly = upperBand;
                    lowerOnly = null;
                    isBullish = false;
                }
                else
                {
                    superTrend = lowerBand;
                    lowerOnly = lowerBand;
                    upperOnly = null;
                    isBullish = true;
                }
            }
            else
            {
                superTrend = null;
                upperOnly = null;
                lowerOnly = null;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                SuperTrend: (decimal?)superTrend,
                UpperBand: (decimal?)upperOnly,
                LowerBand: (decimal?)lowerOnly));
        }

        return results;
    }
}
