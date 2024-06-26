namespace Skender.Stock.Indicators;

// FISHER TRANSFORM (SERIES)

public static partial class Indicator
{
    internal static List<FisherTransformResult> CalcFisherTransform<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusableResult
    {
        // check parameter arguments
        FisherTransform.Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        double[] pr = new double[length]; // median price
        double[] xv = new double[length]; // price transform "value"
        List<FisherTransformResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < source.Count; i++)
        {
            var s = source[i];
            pr[i] = s.Value;

            double minPrice = pr[i];
            double maxPrice = pr[i];

            for (int p = Math.Max(i - lookbackPeriods + 1, 0); p <= i; p++)
            {
                minPrice = Math.Min(pr[p], minPrice);
                maxPrice = Math.Max(pr[p], maxPrice);
            }

            FisherTransformResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            if (i > 0)
            {
                xv[i] = maxPrice != minPrice
                    ? (0.33 * 2 * (((pr[i] - minPrice) / (maxPrice - minPrice)) - 0.5))
                          + (0.67 * xv[i - 1])
                    : 0;

                xv[i] = (xv[i] > 0.99) ? 0.999 : xv[i];
                xv[i] = (xv[i] < -0.99) ? -0.999 : xv[i];

                r.Fisher = ((0.5 * Math.Log((1 + xv[i]) / (1 - xv[i])))
                      + (0.5 * results[i - 1].Fisher)).NaN2Null();

                r.Trigger = results[i - 1].Fisher;
            }
            else
            {
                xv[i] = 0;
                r.Fisher = 0;
            }
        }

        return results;
    }
}
