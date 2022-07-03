namespace Skender.Stock.Indicators;

// ULCER INDEX (SERIES)
public static partial class Indicator
{
    internal static List<UlcerIndexResult> CalcUlcerIndex(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateUlcer(lookbackPeriods);

        // initialize
        List<UlcerIndexResult> results = new(tpList.Count);

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double _) = tpList[i];

            UlcerIndexResult r = new(date);
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double sumSquared = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    int dIndex = p + 1;

                    double maxClose = 0;
                    for (int s = i + 1 - lookbackPeriods; s < dIndex; s++)
                    {
                        (DateTime _, double sValue) = tpList[s];
                        if (sValue > maxClose)
                        {
                            maxClose = sValue;
                        }
                    }

                    double percentDrawdown = (maxClose == 0) ? double.NaN
                        : 100 * ((pValue - maxClose) / maxClose);

                    sumSquared += percentDrawdown * percentDrawdown;
                }

                r.UI = Math.Sqrt(sumSquared / lookbackPeriods).NaN2Null();
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateUlcer(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Ulcer Index.");
        }
    }
}
