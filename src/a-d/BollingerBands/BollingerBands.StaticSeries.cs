namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (SERIES)

public static partial class BollingerBands
{
    public static IReadOnlyList<BollingerBandsResult> ToBollingerBands<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods, standardDeviations);

        // initialize
        int length = source.Count;
        List<BollingerBandsResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
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
