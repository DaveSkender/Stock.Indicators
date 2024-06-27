namespace Skender.Stock.Indicators;

// CORRELATION COEFFICIENT (SERIES)

public static partial class Indicator
{
    internal static List<CorrResult> CalcCorrelation<T>(
        this List<T> sourceA,
        List<T> sourceB,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Correlation.Validate(sourceA, sourceB, lookbackPeriods);

        // initialize
        int length = sourceA.Count;
        List<CorrResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T a = sourceA[i];
            T b = sourceB[i];

            if (a.Timestamp != b.Timestamp)
            {
                throw new InvalidQuotesException(nameof(sourceA), a.Timestamp,
                    "Timestamp sequence does not match.  Correlation requires matching dates in provided histories.");
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
                r = new CorrResult(Timestamp: a.Timestamp);
            }

            results.Add(r);
        }

        return results;
    }

    // calculate correlation
    private static CorrResult PeriodCorrelation(
        DateTime timestamp,
        double[] dataA,
        double[] dataB)
    {
        int length = dataA.Length;
        double sumA = 0;
        double sumB = 0;
        double sumA2 = 0;
        double sumB2 = 0;
        double sumAB = 0;

        for (int i = 0; i < length; i++)
        {
            double a = dataA[i];
            double b = dataB[i];

            sumA += a;
            sumB += b;
            sumA2 += a * a;
            sumB2 += b * b;
            sumAB += a * b;
        }

        double avgA = sumA / length;
        double avgB = sumB / length;
        double avgA2 = sumA2 / length;
        double avgB2 = sumB2 / length;
        double avgAB = sumAB / length;

        double varA = avgA2 - (avgA * avgA);
        double varB = avgB2 - (avgB * avgB);
        double cov = avgAB - (avgA * avgB);
        double divisor = Math.Sqrt(varA * varB);

        double? corr = (divisor == 0)
            ? null
            : (cov / divisor).NaN2Null();

        return new CorrResult(
            Timestamp: timestamp,
            VarianceA: varA.NaN2Null(),
            VarianceB: varB.NaN2Null(),
            Covariance: cov.NaN2Null(),
            Correlation: corr,
            RSquared: corr * corr);
    }
}
