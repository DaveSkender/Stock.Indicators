namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the correlation coefficient on a series of quotes.
/// </summary>
public static partial class Correlation
{
    /// <summary>
    /// Calculates the correlation coefficient for two series of quotes.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source lists, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="sourceA">The first source list of quotes.</param>
    /// <param name="sourceB">The second source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    /// <returns>A read-only list of <see cref="CorrResult"/> containing the correlation calculation results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either sourceA or sourceB is null.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when the timestamps of sourceA and sourceB do not match.</exception>
    [Series("CORR", "Correlation", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<CorrResult> ToCorrelation<T>(
        //TODO: [ParamNum<IEnumerable<T>>("Source A")]
        this IReadOnlyList<T> sourceA,
        //TODO: [ParamNum<IEnumerable<T>>("Source B")]
        IReadOnlyList<T> sourceB,
        [ParamNum<int>("Lookback Periods", 20, 1, 250)]
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(sourceA);
        ArgumentNullException.ThrowIfNull(sourceB);
        Validate(sourceA, sourceB, lookbackPeriods);

        // initialize
        int length = sourceA.Count;
        List<CorrResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T a = sourceA[i];
            T b = sourceB[i];

            if (a.Timestamp != b.Timestamp)
            {
                throw new InvalidQuotesException(
                    nameof(sourceA), a.Timestamp,
                    "Timestamp sequence does not match.  " +
                    "Correlation requires matching dates in provided histories.");
            }

            CorrResult r;

            // calculate correlation
            if (i >= lookbackPeriods - 1)
            {
                double[] dataA = new double[lookbackPeriods];
                double[] dataB = new double[lookbackPeriods];
                int z = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    dataA[z] = sourceA[p].Value;
                    dataB[z] = sourceB[p].Value;

                    z++;
                }

                r = PeriodCorrelation(a.Timestamp, dataA, dataB);
            }
            else
            {
                r = new(Timestamp: a.Timestamp);
            }

            results.Add(r);
        }

        return results;
    }

    /// <summary>
    /// Calculates the correlation for a given period.
    /// </summary>
    /// <param name="timestamp">The timestamp of the result.</param>
    /// <param name="dataA">The data series A.</param>
    /// <param name="dataB">The data series B.</param>
    /// <returns>A <see cref="CorrResult"/> containing the correlation calculation result for the given period.</returns>
    internal static CorrResult PeriodCorrelation(
        DateTime timestamp,
        double[] dataA,
        double[] dataB)
    {
        int length = dataA.Length;
        double sumA = 0;
        double sumB = 0;
        double sumA2 = 0;
        double sumB2 = 0;
        double sumAb = 0;

        for (int i = 0; i < length; i++)
        {
            double a = dataA[i];
            double b = dataB[i];

            sumA += a;
            sumB += b;
            sumA2 += a * a;
            sumB2 += b * b;
            sumAb += a * b;
        }

        double avgA = sumA / length;
        double avgB = sumB / length;
        double avgA2 = sumA2 / length;
        double avgB2 = sumB2 / length;
        double avgAb = sumAb / length;

        double varA = avgA2 - (avgA * avgA);
        double varB = avgB2 - (avgB * avgB);
        double cov = avgAb - (avgA * avgB);
        double divisor = Math.Sqrt(varA * varB);

        double? corr = divisor == 0
            ? null
            : (cov / divisor).NaN2Null();

        return new(
            Timestamp: timestamp,
            VarianceA: varA.NaN2Null(),
            VarianceB: varB.NaN2Null(),
            Covariance: cov.NaN2Null(),
            Correlation: corr,
            RSquared: corr * corr);
    }
}
