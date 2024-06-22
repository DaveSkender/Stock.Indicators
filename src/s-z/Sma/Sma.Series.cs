namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (SERIES)

public static partial class Indicator
{
    // TODO: discontinue converting to Tuple,
    // everywhere.  It's unneeded overhead; use IReusableResult.

    internal static List<SmaResult> CalcSma(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // check parameter arguments
        SmaUtilities.Validate(lookbackPeriods);

        // initialize
        List<SmaResult> results = new(tpList.Count);

        // roll through quotes
        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime date, double _) = tpList[i];

            double sma;

            if (i >= lookbackPeriods - 1)
            {
                double sumSma = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    (DateTime _, double pValue) = tpList[p];
                    sumSma += pValue;
                }

                sma = sumSma / lookbackPeriods;
            }
            else
            {
                sma = double.NaN;
            }

            SmaResult result = new(
                Timestamp: date,
                Sma: sma.NaN2Null());

            results.Add(result);
        }

        return results;
    }
}
