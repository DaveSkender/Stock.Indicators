namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Double Exponential Moving Average (DEMA) on a series of data.
/// </summary>
public static partial class Dema
{
    /// <summary>
    /// Calculates the Double Exponential Moving Average (DEMA) for a series of data.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source list, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The source list of data points.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback.</param>
    /// <returns>A list of <see cref="DemaResult"/> containing the DEMA values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are not valid.</exception>
    public static IReadOnlyList<DemaResult> ToDema<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<DemaResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double lastEma1 = double.NaN;
        double lastEma2 = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                results.Add(new(s.Timestamp));
                continue;
            }

            double ema1;
            double ema2;

            // when no prior EMA, reset as SMA
            if (double.IsNaN(lastEma2))
            {
                double sum = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                ema1 = ema2 = sum / lookbackPeriods;
            }

            // normal DEMA
            else
            {
                ema1 = lastEma1 + (k * (s.Value - lastEma1));
                ema2 = lastEma2 + (k * (ema1 - lastEma2));
            }

            results.Add(new DemaResult(
                Timestamp: s.Timestamp,
                Dema: ((2d * ema1) - ema2).NaN2Null()));

            lastEma1 = ema1;
            lastEma2 = ema2;
        }

        return results;
    }
}
