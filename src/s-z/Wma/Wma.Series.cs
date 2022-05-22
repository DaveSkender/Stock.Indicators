namespace Skender.Stock.Indicators;

// WEIGHTED MOVING AVERAGE (SERIES)
public static partial class Indicator
{
    // series calculation
    internal static IEnumerable<WmaResult> CalcWma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        ValidateWma(lookbackPeriods);

        // initialize
        List<WmaResult> results = new(tpList.Count);
        double divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double value) = tpList[i];

            WmaResult result = new()
            {
                Date = date
            };

            if (i + 1 >= lookbackPeriods)
            {
                double? wma = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    (DateTime pDate, double pValue) = tpList[p];
                    wma += pValue * (lookbackPeriods - (i + 1 - p - 1)) / divisor;
                }

                result.Wma = wma;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateWma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for WMA.");
        }
    }
}
