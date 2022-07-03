namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (ANALYSIS)
public static partial class Indicator
{
    internal static IEnumerable<SmaAnalysis> CalcSmaAnalysis(
        this List<(DateTime, double)> tpList,
        int lookbackPeriods)
    {
        // initialize
        List<SmaAnalysis>? results = tpList
            .CalcSma(lookbackPeriods)
            .Select(x => new SmaAnalysis(x.Date) { Sma = x.Sma })
            .ToList();

        // roll through quotes
        for (int i = lookbackPeriods - 1; i < results.Count; i++)
        {
            SmaAnalysis r = results[i];
            double sma = (r.Sma == null) ? double.NaN : (double)r.Sma;

            double sumMad = 0;
            double sumMse = 0;
            double sumMape = 0;

            for (int p = i + 1 - lookbackPeriods; p <= i; p++)
            {
                (DateTime _, double value) = tpList[p];

                sumMad += Math.Abs(value - sma);
                sumMse += (value - sma) * (value - sma);

                sumMape += (value == 0) ? double.NaN
                    : Math.Abs(value - sma) / value;
            }

            // mean absolute deviation
            r.Mad = (sumMad / lookbackPeriods).NaN2Null();

            // mean squared error
            r.Mse = (sumMse / lookbackPeriods).NaN2Null();

            // mean absolute percent error
            r.Mape = (sumMape / lookbackPeriods).NaN2Null();
        }

        return results;
    }
}
