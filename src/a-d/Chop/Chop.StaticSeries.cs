namespace Skender.Stock.Indicators;

/// <summary>
/// Choppiness Index (CHOP) on a series of quotes indicator.
/// </summary>
public static partial class Chop
{
    /// <summary>
    /// Calculates the Choppiness Index (CHOP) for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A read-only list of <see cref="ChopResult"/> containing the CHOP calculation results.</returns>
    public static IReadOnlyList<ChopResult> ToChop(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
        => quotes
            .ToQuoteDList()
            .CalcChop(lookbackPeriods);

    /// <summary>
    /// Calculates the Choppiness Index (CHOP) for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of <see cref="ChopResult"/> containing the CHOP calculation results.</returns>
    private static List<ChopResult> CalcChop(
        this List<QuoteD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = quotes.Count;
        List<ChopResult> results = new(length);
        double[] trueHigh = new double[length];
        double[] trueLow = new double[length];
        double[] trueRange = new double[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            double? chop = null;

            if (i > 0)
            {
                trueHigh[i] = Math.Max(quotes[i].High, quotes[i - 1].Close);
                trueLow[i] = Math.Min(quotes[i].Low, quotes[i - 1].Close);
                trueRange[i] = trueHigh[i] - trueLow[i];

                // calculate CHOP

                if (i >= lookbackPeriods)
                {
                    // reset measurements
                    double sum = trueRange[i];
                    double high = trueHigh[i];
                    double low = trueLow[i];

                    // iterate over lookback window
                    for (int j = 1; j < lookbackPeriods; j++)
                    {
                        sum += trueRange[i - j];
                        high = Math.Max(high, trueHigh[i - j]);
                        low = Math.Min(low, trueLow[i - j]);
                    }

                    double range = high - low;

                    // calculate CHOP
                    if (range != 0)
                    {
                        chop = 100 * (Math.Log(sum / range) / Math.Log(lookbackPeriods));
                    }
                }
            }

            results.Add(new(
                Timestamp: quotes[i].Timestamp,
                Chop: chop));
        }

        return results;
    }
}
