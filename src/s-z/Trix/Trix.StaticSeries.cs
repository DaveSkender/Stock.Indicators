namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Triple Exponential Moving Average (TRIX) oscillator.
/// </summary>
public static partial class Trix
{
    /// <summary>
    /// Converts a source list to a list of TRIX results.
    /// </summary>
    /// <typeparam name="T">The type of the source elements, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The source list of elements.</param>
    /// <param name="lookbackPeriods">The number of lookback periods for the TRIX calculation.</param>
    /// <returns>A read-only list of <see cref="TrixResult"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    [Series("TRIX", "Triple Exponential Moving Average Oscillator", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<TrixResult> ToTrix<T>(
        this IReadOnlyList<T> source,

        [Param("Lookback Periods", 1, 250, 14)]
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<TrixResult> results = new(length);

        double k = 2d / (lookbackPeriods + 1);
        double lastEma1 = double.NaN;
        double lastEma2 = double.NaN;
        double lastEma3 = double.NaN;

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
            double ema3;

            // when no prior EMA, reset as SMA
            if (double.IsNaN(lastEma3))
            {
                double sum = 0;
                for (int p = i - lookbackPeriods + 1; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                ema1 = ema2 = ema3 = sum / lookbackPeriods;

                results.Add(new(s.Timestamp));
            }

            // normal TRIX
            else
            {
                ema1 = lastEma1 + (k * (s.Value - lastEma1));
                ema2 = lastEma2 + (k * (ema1 - lastEma2));
                ema3 = lastEma3 + (k * (ema2 - lastEma3));

                double trix = 100 * (ema3 - lastEma3) / lastEma3;

                results.Add(new TrixResult(
                    Timestamp: s.Timestamp,
                    Ema3: ema3.NaN2Null(),
                    Trix: trix.NaN2Null()));
            }

            lastEma1 = ema1;
            lastEma2 = ema2;
            lastEma3 = ema3;
        }

        return results;
    }
}
