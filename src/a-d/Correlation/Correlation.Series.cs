namespace Skender.Stock.Indicators;

// CORRELATION COEFFICIENT (SERIES)
public static partial class Indicator
{
    internal static IEnumerable<CorrResult> CalcCorrelation(
        this List<(DateTime, double)> tpListA,
        List<(DateTime, double)> tpListB,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateCorrelation(tpListA, tpListB, lookbackPeriods);

        // initialize
        int length = tpListA.Count;
        List<CorrResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            (DateTime aDate, double aValue) = tpListA[i];
            (DateTime bDate, double bValue) = tpListB[i];

            if (aDate != bDate)
            {
                throw new InvalidQuotesException(nameof(tpListA), aDate,
                    "Date sequence does not match.  Correlation requires matching dates in provided histories.");
            }

            CorrResult r = new()
            {
                Date = aDate
            };

            // calculate correlation
            if (i >= lookbackPeriods - 1)
            {
                double[] dataA = new double[lookbackPeriods];
                double[] dataB = new double[lookbackPeriods];
                int z = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    dataA[z] = tpListA[p].Item2;
                    dataB[z] = tpListB[p].Item2;

                    z++;
                }

                r.PeriodCorrelation(dataA, dataB);
            }

            results.Add(r);
        }

        return results;
    }

    // calculate correlation
    private static void PeriodCorrelation(
        this CorrResult r,
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

        r.VarianceA = varA;
        r.VarianceB = varB;
        r.Covariance = cov;
        r.Correlation = (divisor == 0) ? double.NaN : cov / divisor;
        r.RSquared = r.Correlation * r.Correlation;
    }

    // parameter validation
    private static void ValidateCorrelation(
        List<(DateTime, double)> quotesA,
        List<(DateTime, double)> quotesB,
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Correlation.");
        }

        // check quotes
        if (quotesA.Count != quotesB.Count)
        {
            throw new InvalidQuotesException(
                nameof(quotesB),
                "B quotes should have at least as many records as A quotes for Correlation.");
        }
    }
}