namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Ultimate Oscillator indicator.
/// </summary>
public static partial class Ultimate
{
    /// <summary>
    /// Calculates the Ultimate Oscillator for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the source list, which must implement IQuote.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="shortPeriods">The number of short lookback periods. Default is 7.</param>
    /// <param name="middlePeriods">The number of middle lookback periods. Default is 14.</param>
    /// <param name="longPeriods">The number of long lookback periods. Default is 28.</param>
    /// <returns>A list of UltimateResult containing the Ultimate Oscillator values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static IReadOnlyList<UltimateResult> ToUltimate<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcUltimate(shortPeriods, middlePeriods, longPeriods);

    /// <summary>
    /// Calculates the Ultimate Oscillator for a series of quotes.
    /// </summary>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="shortPeriods">The number of short lookback periods.</param>
    /// <param name="middlePeriods">The number of middle lookback periods.</param>
    /// <param name="longPeriods">The number of long lookback periods.</param>
    /// <returns>A list of UltimateResult containing the Ultimate Oscillator values.</returns>
    private static List<UltimateResult> CalcUltimate(
        this List<QuoteD> source,
        int shortPeriods,
        int middlePeriods,
        int longPeriods)
    {
        // check parameter arguments
        Validate(shortPeriods, middlePeriods, longPeriods);

        // initialize
        int length = source.Count;
        List<UltimateResult> results = new(length);
        double[] bp = new double[length]; // buying pressure
        double[] tr = new double[length]; // true range

        double priorClose = 0;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];
            double? ultimate;

            if (i > 0)
            {
                bp[i] = q.Close - Math.Min(q.Low, priorClose);
                tr[i] = Math.Max(q.High, priorClose) - Math.Min(q.Low, priorClose);
            }

            if (i >= longPeriods)
            {
                double sumBp1 = 0;
                double sumBp2 = 0;
                double sumBp3 = 0;

                double sumTr1 = 0;
                double sumTr2 = 0;
                double sumTr3 = 0;

                for (int p = i + 1 - longPeriods; p <= i; p++)
                {
                    int pIndex = p + 1;

                    // short aggregate
                    if (pIndex > i + 1 - shortPeriods)
                    {
                        sumBp1 += bp[p];
                        sumTr1 += tr[p];
                    }

                    // middle aggregate
                    if (pIndex > i + 1 - middlePeriods)
                    {
                        sumBp2 += bp[p];
                        sumTr2 += tr[p];
                    }

                    // long aggregate
                    sumBp3 += bp[p];
                    sumTr3 += tr[p];
                }

                double avg1 = sumTr1 == 0 ? double.NaN : sumBp1 / sumTr1;
                double avg2 = sumTr2 == 0 ? double.NaN : sumBp2 / sumTr2;
                double avg3 = sumTr3 == 0 ? double.NaN : sumBp3 / sumTr3;

                ultimate = (100d * ((4d * avg1) + (2d * avg2) + avg3) / 7d).NaN2Null();
            }
            else
            {
                ultimate = null;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Ultimate: ultimate));

            priorClose = q.Close;
        }

        return results;
    }
}
