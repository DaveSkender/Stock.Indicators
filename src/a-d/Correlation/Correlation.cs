namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CORRELATION COEFFICIENT
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<CorrResult> GetCorrelation<TQuote>(
        this IEnumerable<TQuote> quotesA,
        IEnumerable<TQuote> quotesB,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdListA = quotesA.ToBasicClass(CandlePart.Close);
        List<BasicData> bdListB = quotesB.ToBasicClass(CandlePart.Close);

        // check parameter arguments
        ValidateCorrelation(quotesA, quotesB, lookbackPeriods);

        // initialize
        List<CorrResult> results = new(bdListA.Count);

        // roll through quotes
        for (int i = 0; i < bdListA.Count; i++)
        {
            BasicData a = bdListA[i];
            BasicData b = bdListB[i];

            if (a.Date != b.Date)
            {
                throw new InvalidQuotesException(nameof(quotesA), a.Date,
                    "Date sequence does not match.  Correlation requires matching dates in provided histories.");
            }

            CorrResult r = new()
            {
                Date = a.Date
            };

            // calculate correlation
            if (i + 1 >= lookbackPeriods)
            {
                double[] dataA = new double[lookbackPeriods];
                double[] dataB = new double[lookbackPeriods];
                int z = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    dataA[z] = bdListA[p].Value;
                    dataB[z] = bdListB[p].Value;

                    z++;
                }

                r.CalcCorrelation(dataA, dataB);
            }

            results.Add(r);
        }

        return results;
    }

    // calculate correlation
    private static void CalcCorrelation(
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
    private static void ValidateCorrelation<TQuote>(
        IEnumerable<TQuote> quotesA,
        IEnumerable<TQuote> quotesB,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Correlation.");
        }

        // check quotes
        if (quotesA.Count() != quotesB.Count())
        {
            throw new InvalidQuotesException(
                nameof(quotesB),
                "B quotes should have at least as many records as A quotes for Correlation.");
        }
    }
}
