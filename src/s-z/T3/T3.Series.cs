namespace Skender.Stock.Indicators;

// TILLSON T3 MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<T3Result> CalcT3<T>(
        this List<T> source,
        int lookbackPeriods,
        double volumeFactor)
        where T : IReusableResult
    {
        // check parameter arguments
        T3.Validate(lookbackPeriods, volumeFactor);

        // initialize
        int length = source.Count;
        List<T3Result> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double a = volumeFactor;

        double c1 = -a * a * a;
        double c2 = (3 * a * a) + (3 * a * a * a);
        double c3 = (-6 * a * a) - (3 * a) - (3 * a * a * a);
        double c4 = 1 + (3 * a) + (a * a * a) + (3 * a * a);

        double e1 = double.NaN;
        double e2 = double.NaN;
        double e3 = double.NaN;
        double e4 = double.NaN;
        double e5 = double.NaN;
        double e6 = double.NaN;

        // roll through remaining quotes
        for (int i = 0; i < length; i++)
        {
            var s = source[i];
            T3Result r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            // re/seed values
            if (double.IsNaN(e6))
            {
                e1 = e2 = e3 = e4 = e5 = e6 = s.Value;
            }

            // first smoothing
            e1 += k * (s.Value - e1);
            e2 += k * (e1 - e2);
            e3 += k * (e2 - e3);
            e4 += k * (e3 - e4);
            e5 += k * (e4 - e5);
            e6 += k * (e5 - e6);

            // T3 moving average
            r.T3 = ((c1 * e6) + (c2 * e5) + (c3 * e4) + (c4 * e3)).NaN2Null();
        }

        return results;
    }
}
