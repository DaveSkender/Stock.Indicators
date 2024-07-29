namespace Skender.Stock.Indicators;

// AWESOME OSCILLATOR (SERIES)

public static partial class Indicator
{
    private static List<AwesomeResult> CalcAwesome<T>(
        this List<T> source,
        int fastPeriods,
        int slowPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Awesome.Validate(fastPeriods, slowPeriods);

        // initialize
        int length = source.Count;
        List<AwesomeResult> results = new(length);
        double[] pr = new double[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable s = source[i];
            pr[i] = s.Value;

            double? oscillator = null;
            double? normalized = null;

            if (i >= slowPeriods - 1)
            {
                double sumSlow = 0;
                double sumFast = 0;

                for (int p = i + 1 - slowPeriods; p <= i; p++)
                {
                    sumSlow += pr[p];

                    if (p >= i + 1 - fastPeriods)
                    {
                        sumFast += pr[p];
                    }
                }

                oscillator = ((sumFast / fastPeriods) - (sumSlow / slowPeriods)).NaN2Null();
                normalized = pr[i] != 0 ? 100 * oscillator / pr[i] : null;
            }

            AwesomeResult r = new(
                Timestamp: s.Timestamp,
                Oscillator: oscillator,
                Normalized: normalized);

            results.Add(r);
        }

        return results;
    }
}
