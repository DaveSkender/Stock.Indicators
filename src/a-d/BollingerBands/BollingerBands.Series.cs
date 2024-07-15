namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (SERIES)

public static partial class Indicator
{
    private static List<BollingerBandsResult> CalcBollingerBands<T>(
        this List<T> source,
        int lookbackPeriods,
        double standardDeviations)
        where T : IReusable
    {
        // check parameter arguments
        BollingerBands.Validate(lookbackPeriods, standardDeviations);

        // initialize
        List<BollingerBandsResult> results = new(source.Count);

        // roll through source values
        for (int i = 0; i < source.Count; i++)
        {
            T s = source[i];

            if (i >= lookbackPeriods - 1)
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

                double? sma = (sum / lookbackPeriods).NaN2Null();
                double? stdDev = window.StdDev().NaN2Null();

                double? upperBand = sma + (standardDeviations * stdDev);
                double? lowerBand = sma - (standardDeviations * stdDev);

                results.Add(new BollingerBandsResult(

                    Timestamp: s.Timestamp,

                    Sma: sma,
                    UpperBand: upperBand,
                    LowerBand: lowerBand,

                    PercentB: upperBand - lowerBand == 0 ? null
                        : (s.Value - lowerBand) / (upperBand - lowerBand),

                    ZScore: stdDev == 0 ? null : (s.Value - sma) / stdDev,
                    Width: sma == 0 ? null : (upperBand - lowerBand) / sma
                ));
            }

            // initization period
            else
            {
                results.Add(new(s.Timestamp));
            }
        }

        return results;
    }
}
