namespace Skender.Stock.Indicators;

// FISHER TRANSFORM (SERIES)

public static partial class FisherTransform
{
    public static IReadOnlyList<FisherTransformResult> ToFisherTransform<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 10)
        where T : IReusable
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // prefer HL2 when IQuote
        IReadOnlyList<IReusable> values
            = source.ToPreferredList(CandlePart.HL2);

        // initialize
        int length = values.Count;
        double[] pr = new double[length]; // median price
        double[] xv = new double[length]; // price transform "value"
        List<FisherTransformResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable s = values[i];
            pr[i] = s.Value;

            double minPrice = pr[i];
            double maxPrice = pr[i];

            for (int p = Math.Max(i - lookbackPeriods + 1, 0); p <= i; p++)
            {
                minPrice = Math.Min(pr[p], minPrice);
                maxPrice = Math.Max(pr[p], maxPrice);
            }

            double? fisher;
            double? trigger = null;

            if (i > 0)
            {
                xv[i] = maxPrice - minPrice != 0
                    ? (0.33 * 2 * (((pr[i] - minPrice) / (maxPrice - minPrice)) - 0.5))
                          + (0.67 * xv[i - 1])
                    : 0;

                xv[i] = xv[i] > 0.99 ? 0.999 : xv[i];
                xv[i] = xv[i] < -0.99 ? -0.999 : xv[i];

                fisher = ((0.5 * Math.Log((1 + xv[i]) / (1 - xv[i])))
                      + (0.5 * results[i - 1].Fisher)).NaN2Null();

                trigger = results[i - 1].Fisher;
            }
            else
            {
                xv[i] = 0;
                fisher = 0;
            }

            results.Add(new(
                Timestamp: s.Timestamp,
                Trigger: trigger,
                Fisher: fisher));
        }

        return results;
    }
}