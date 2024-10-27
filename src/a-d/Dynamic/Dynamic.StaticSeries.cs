namespace Skender.Stock.Indicators;

// McGINLEY DYNAMIC (SERIES)

public static partial class MgDynamic
{
    public static IReadOnlyList<DynamicResult> ToDynamic<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods,
        double kFactor = 0.6)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods, kFactor);

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
