namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC (SERIES)

public static partial class Indicator
{
    internal static List<DynamicResult> CalcDynamic(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods,
        double kFactor)
    {
        // check parameter arguments
        MgDynamic.Validate(lookbackPeriods, kFactor);

        // initialize
        int length = tpList.Count;
        List<DynamicResult> results = new(length);

        double prevDyn = double.NaN;

        // roll through quotes, to get preliminary data
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            DynamicResult r = new() { Date = date };
            results.Add(r);

            // re/initialize
            if (double.IsNaN(prevDyn))
            {
                prevDyn = value;
            }

            // normal Dynamic
            else
            {
                double dyn = prevDyn + ((value - prevDyn) /
                   (kFactor * lookbackPeriods * Math.Pow(value / prevDyn, 4)));

                r.Dynamic = dyn.NaN2Null();
                prevDyn = dyn;
            }
        }

        return results;
    }
}
