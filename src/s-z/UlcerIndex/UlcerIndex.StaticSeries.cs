namespace Skender.Stock.Indicators;

// ULCER INDEX (SERIES)

public static partial class UlcerIndex
{
    public static IReadOnlyList<UlcerIndexResult> ToUlcerIndex<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 14)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<UlcerIndexResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double? ui;

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

                    double percentDrawdown = maxClose == 0 ? double.NaN
                        : 100 * ((ps.Value - maxClose) / maxClose);

                    sumSquared += percentDrawdown * percentDrawdown;
                }

                ui = Math.Sqrt(sumSquared / lookbackPeriods).NaN2Null();
            }
            else
            {
                ui = null;
            }

            UlcerIndexResult r = new(
                Timestamp: s.Timestamp,
                UlcerIndex: ui);
            results.Add(r);
        }

        return results;
    }
}
