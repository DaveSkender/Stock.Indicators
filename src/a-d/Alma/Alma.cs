namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ARNAUD LEGOUX MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<AlmaResult> GetAlma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ToBasicD(CandlePart.Close);

        // check parameter arguments
        ValidateAlma(lookbackPeriods, offset, sigma);

        // initialize
        List<AlmaResult> results = new(bdList.Count);

        // determine price weights
        double m = offset * (lookbackPeriods - 1);
        double s = lookbackPeriods / sigma;

        double[] weight = new double[lookbackPeriods];
        double norm = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double wt = Math.Exp(-((i - m) * (i - m)) / (2 * s * s));
            weight[i] = wt;
            norm += wt;
        }

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicD q = bdList[i];
            int index = i + 1;

            AlmaResult r = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                double weightedSum = 0;
                int n = 0;

                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    BasicD d = bdList[p];
                    weightedSum += weight[n] * d.Value;
                    n++;
                }

                r.Alma = (decimal)(weightedSum / norm);
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateAlma(
        int lookbackPeriods,
        double offset,
        double sigma)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for ALMA.");
        }

        if (offset is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), offset,
                "Offset must be between 0 and 1 for ALMA.");
        }

        if (sigma <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sigma), sigma,
                "Sigma must be greater than 0 for ALMA.");
        }
    }
}
