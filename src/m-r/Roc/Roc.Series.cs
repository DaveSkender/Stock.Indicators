namespace Skender.Stock.Indicators;

// RATE OF CHANGE (SERIES)

public static partial class Indicator
{
    internal static List<RocResult> CalcRoc<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        Roc.Validate(lookbackPeriods);

        // initialize
        List<RocResult> results = new(source.Count);

        // roll through quotes
        for (int i = 0; i < source.Count; i++)
        {
            var s = source[i];

            RocResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            if (i + 1 > lookbackPeriods)
            {
                var back = source[i - lookbackPeriods];

                r.Momentum = (s.Value - back.Value).NaN2Null();
                r.Roc = (back.Value == 0) ? null
                    : (100d * r.Momentum / back.Value).NaN2Null();
            }
        }

        return results;
    }
}
