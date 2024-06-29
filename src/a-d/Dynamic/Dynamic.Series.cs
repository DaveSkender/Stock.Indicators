namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC (SERIES)

public static partial class Indicator
{
    internal static List<DynamicResult> CalcDynamic<T>(
        this List<T> source,
        int lookbackPeriods,
        double kFactor)
        where T : IReusable
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
            T s = source[i];
            double dyn;

            // re/initialize
            if (double.IsNaN(prevDyn))
            {
                dyn = double.NaN;
                prevDyn = s.Value;
            }

            // normal Dynamic
            else
            {
                dyn = prevDyn + ((s.Value - prevDyn) /
                   (kFactor * lookbackPeriods * Math.Pow(s.Value / prevDyn, 4)));

                prevDyn = dyn;
            }

            results.Add(new DynamicResult(
                Timestamp: s.Timestamp,
                Dynamic: dyn.NaN2Null()));
        }

        return results;
    }
}
