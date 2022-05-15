namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // FISHER TRANSFORM
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<FisherTransformResult> GetFisherTransform<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 10)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(CandlePart.HL2);

        // check parameter arguments
        ValidateFisherTransform(lookbackPeriods);

        // initialize
        int length = bdList.Count;
        double[] pr = new double[length]; // median price
        double[] xv = new double[length]; // price transform "value"
        List<FisherTransformResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicData q = bdList[i];
            pr[i] = q.Value;

            double minPrice = pr[i];
            double maxPrice = pr[i];

            for (int p = Math.Max(i - lookbackPeriods + 1, 0); p <= i; p++)
            {
                minPrice = Math.Min(pr[p], minPrice);
                maxPrice = Math.Max(pr[p], maxPrice);
            }

            FisherTransformResult r = new()
            {
                Date = q.Date
            };

            if (i > 0)
            {
                xv[i] = maxPrice != minPrice
                    ? (0.33 * 2 * (((pr[i] - minPrice) / (maxPrice - minPrice)) - 0.5))
                          + (0.67 * xv[i - 1])
                    : 0;

                xv[i] = (xv[i] > 0.99) ? 0.999 : xv[i];
                xv[i] = (xv[i] < -0.99) ? -0.999 : xv[i];

                r.Fisher = (0.5 * Math.Log((1 + xv[i]) / (1 - xv[i])))
                      + (0.5 * results[i - 1].Fisher);

                r.Trigger = results[i - 1].Fisher;
            }
            else
            {
                xv[i] = 0;
                r.Fisher = 0;
            }

            results.Add(r);
        }

        return results;
    }

    // parameter validation
    private static void ValidateFisherTransform(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Fisher Transform.");
        }
    }
}
