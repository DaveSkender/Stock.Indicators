namespace Skender.Stock.Indicators;

// ULCER INDEX (SERIES)

public static partial class Indicator
{
    internal static List<UlcerIndexResult> CalcUlcerIndex<T>(
        this List<T> source,
        int lookbackPeriods)
        where T : IReusableResult
    {
        // check parameter arguments
        UlcerIndex.Validate(lookbackPeriods);

        // initialize
        List<UlcerIndexResult> results = new(source.Count);

        // roll through quotes
        for (int i = 0; i < source.Count; i++)
        {
            T s = source[i];

            UlcerIndexResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double sumSquared = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    int dIndex = p + 1;

                    double maxClose = 0;
                    for (int z = i + 1 - lookbackPeriods; z < dIndex; z++)
                    {
                        T zs = source[z];
                        if (zs.Value > maxClose)
                        {
                            maxClose = zs.Value;
                        }
                    }

                    double percentDrawdown = (maxClose == 0) ? double.NaN
                        : 100 * ((ps.Value - maxClose) / maxClose);

                    sumSquared += percentDrawdown * percentDrawdown;
                }

                r.UlcerIndex = Math.Sqrt(sumSquared / lookbackPeriods).NaN2Null();
            }
        }

        return results;
    }
}
