namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (SERIES)

public static partial class Indicator
{
    internal static List<BollingerBandsResult> CalcBollingerBands<T>(
        this List<T> source,
        int lookbackPeriods,
        double standardDeviations)
        where T : IReusable
    {
        // check parameter arguments
        BollingerBands.Validate(lookbackPeriods, standardDeviations);

        // initialize
        List<BollingerBandsResult> results = new(source.Count);

        // roll through quotes
        for (int i = 0; i < source.Count; i++)
        {
            T s = source[i];

            BollingerBandsResult r = new() { Timestamp = s.Timestamp };
            results.Add(r);

            if (i + 1 >= lookbackPeriods)
            {
                double[] window = new double[lookbackPeriods];
                double sum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    window[n] = ps.Value;
                    sum += ps.Value;
                    n++;
                }

                double? periodAvg = (sum / lookbackPeriods).NaN2Null();
                double? stdDev = window.StdDev().NaN2Null();

                r.Sma = periodAvg;
                r.UpperBand = periodAvg + (standardDeviations * stdDev);
                r.LowerBand = periodAvg - (standardDeviations * stdDev);

                r.PercentB = (r.UpperBand == r.LowerBand) ? null
                    : (s.Value - r.LowerBand) / (r.UpperBand - r.LowerBand);

                r.ZScore = (stdDev == 0) ? null : (s.Value - r.Sma) / stdDev;
                r.Width = (periodAvg == 0) ? null : (r.UpperBand - r.LowerBand) / periodAvg;
            }
        }

        return results;
    }
}
