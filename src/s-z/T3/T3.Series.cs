namespace Skender.Stock.Indicators;

// TILLSON T3 MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    internal static List<T3Result> CalcT3(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double volumeFactor)
    {
        // check parameter arguments
        ValidateT3(lookbackPeriods, volumeFactor);

        // initialize
        int length = tpList.Count;
        List<T3Result> results = new(length);

        if (length == 0)
        {
            return results;
        }

        double k = 2d / (lookbackPeriods + 1);
        double a = volumeFactor;
        double c1 = -a * a * a;
        double c2 = (3 * a * a) + (3 * a * a * a);
        double c3 = (-6 * a * a) - (3 * a) - (3 * a * a * a);
        double c4 = 1 + (3 * a) + (a * a * a) + (3 * a * a);

        double? e1;
        double? e2;
        double? e3;
        double? e4;
        double? e5;
        double? e6;

        // add initial value
        (DateTime date, double value) r0 = tpList[0];
        e1 = e2 = e3 = e4 = e5 = e6 = r0.value;
        results.Add(new T3Result(r0.date) { T3 = r0.value });

        // roll through quotes
        for (int i = 1; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];
            T3Result r = new(date);
            results.Add(r);

            // first smoothing
            e1 += k * (value - e1);
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

    // parameter validation
    private static void ValidateT3(
        int lookbackPeriods,
        double volumeFactor)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for T3.");
        }

        if (volumeFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(volumeFactor), volumeFactor,
                "Volume Factor must be greater than 0 for T3.");
        }
    }
}
