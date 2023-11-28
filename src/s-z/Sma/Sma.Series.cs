namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    internal static List<SmaResult> CalcSma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        Sma.Validate(lookbackPeriods);

        // initialize
        List<SmaResult> results = new(tpList.Count);

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double _) = tpList[i];

            SmaResult result = new() { Date = date };
            results.Add(result);

            if (i >= lookbackPeriods - 1)
            {
                double sumSma = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    sumSma += pValue;
                }

                result.Sma = (sumSma / lookbackPeriods).NaN2Null();
            }
        }

        return results;
    }
}