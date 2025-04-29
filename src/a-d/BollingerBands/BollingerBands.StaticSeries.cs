namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating Bollinger Bands.
/// </summary>
public static partial class BollingerBands
{
    /// <summary>
    /// Calculates the Bollinger Bands for a series of data.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source list, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The source list of data.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window. Default is 20.</param>
    /// <param name="standardDeviations">The number of standard deviations to use for the bands. Default is 2.</param>
    /// <returns>A read-only list of <see cref="BollingerBandsResult"/> containing the Bollinger Bands calculation results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    [Series("BB", "Bollinger Bands®", Category.PriceChannel, ChartType.Overlay)]
    public static IReadOnlyList<BollingerBandsResult> ToBollingerBands<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Lookback Periods", 2, 250, 20)]
        int lookbackPeriods = 20,
        [ParamNum<double>("Standard Deviations", 0.01, 10, 2)]
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
