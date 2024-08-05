namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC (SERIES)

public static partial class MgDynamic
{
    private static List<DynamicResult> CalcDynamic<T>(
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

        // skip first period
        if (length > 0)
        {
            results.Add(new(source[0].Timestamp, null));
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            double? dyn = Increment(
                lookbackPeriods,
                kFactor,
                newVal: source[i].Value,
                prevDyn: results[i - 1].Dynamic ?? source[i - 1].Value
                ).NaN2Null();

            results.Add(new DynamicResult(
                Timestamp: source[i].Timestamp,
                Dynamic: dyn));
        }

        return results;
    }
}
