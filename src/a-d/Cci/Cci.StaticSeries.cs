namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Commodity Channel Index (CCI) on a series of quotes.
/// </summary>
public static partial class Cci
{
    /// <summary>
    /// Calculates the Commodity Channel Index (CCI) for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the quotes list, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window. Default is 20.</param>
    /// <returns>A read-only list of <see cref="CciResult"/> containing the CCI calculation results.</returns>
    [Series("CCI", "Commodity Channel Index (CCI)", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<CciResult> ToCci<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [ParamNum<int>("Lookback Periods", 20, 1, 250)]
        int lookbackPeriods = 20)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcCci(lookbackPeriods);

    /// <summary>
    /// Calculates the Commodity Channel Index (CCI) for a series of quotes.
    /// </summary>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    /// <returns>A list of <see cref="CciResult"/> containing the CCI calculation results.</returns>
    private static List<CciResult> CalcCci(
        this List<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<CciResult> results = new(length);
        double[] tp = new double[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];
            tp[i] = (q.High + q.Low + q.Close) / 3d;

            double? cci = null;

            if (i + 1 >= lookbackPeriods)
            {
                // average TP over lookback
                double avgTp = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    avgTp += tp[p];
                }

                avgTp /= lookbackPeriods;

                // average Deviation over lookback
                double avgDv = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    avgDv += Math.Abs(avgTp - tp[p]);
                }

                avgDv /= lookbackPeriods;

                cci = avgDv == 0 ? null
                    : ((tp[i] - avgTp) / (0.015 * avgDv)).NaN2Null();
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Cci: cci));
        }

        return results;
    }
}
