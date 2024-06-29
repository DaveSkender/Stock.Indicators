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
        int length = source.Count;
        List<RocResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double roc;
            double momentum;

            if (i + 1 > lookbackPeriods)
            {
                T back = source[i - lookbackPeriods];

                momentum = s.Value - back.Value;

                roc = (back.Value == 0)
                    ? double.NaN
                    : (100d * momentum / back.Value);
            }
            else
            {
                momentum = double.NaN;
                roc = double.NaN;
            }

            RocResult r = new(
                Timestamp: s.Timestamp,
                Momentum: momentum.NaN2Null(),
                Roc: roc.NaN2Null());

            results.Add(r);
        }

        return results;
    }
}
