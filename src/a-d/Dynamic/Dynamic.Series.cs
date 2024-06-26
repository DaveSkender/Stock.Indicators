namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC (SERIES)

public static partial class Indicator
{
    internal static List<DynamicResult> CalcDynamic<T>(
        this List<T> source,
        int lookbackPeriods,
        double kFactor)
        where T : IReusableResult
    {
        // check parameter arguments
        MgDynamic.Validate(lookbackPeriods, kFactor);

        // initialize
        int length = source.Count;
        List<DynamicResult> results = new(length);

        double prevDyn = double.NaN;

        // roll through quotes, to get preliminary data
        for (int i = 0; i < length; i++)
        {
            var s = source[i];

            DynamicResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            // re/initialize
            if (double.IsNaN(prevDyn))
            {
                prevDyn = s.Value;
            }

            // normal Dynamic
            else
            {
                double dyn = prevDyn + ((s.Value - prevDyn) /
                   (kFactor * lookbackPeriods * Math.Pow(s.Value / prevDyn, 4)));

                r.Dynamic = dyn.NaN2Null();
                prevDyn = dyn;
            }
        }

        return results;
    }
}
