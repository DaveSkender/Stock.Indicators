using System.Collections.ObjectModel;

namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (ANALYSIS)
public static partial class Indicator
{
    internal static Collection<SmaAnalysis> CalcSmaAnalysis(
        this Collection<(DateTime, double)> tpColl,
        int lookbackPeriods)
    {
        // initialize
        Collection<SmaAnalysis> results = new();
        Collection<SmaResult> smaResults = tpColl.CalcSma(lookbackPeriods);

        foreach (SmaResult s in smaResults)
        {
            results.Add(new SmaAnalysis(s.Date) { Sma = s.Sma });
        }

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
                (DateTime _, double value) = tpColl[p];

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
