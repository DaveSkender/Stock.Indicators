using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (UTILITIES)

public static partial class Sma
{
    /// <summary>
    /// Simple moving average calculation
    /// </summary>
    /// <param name="values">List of chainable values</param>
    /// <param name="lookbackPeriods">
    /// Window to evaluate, prior to 'endIndex'
    /// </param>
    /// <param name="endIndex">
    /// Index position to evaluate or last position when <see langword="null"/>.
    /// </param>
    /// <typeparam name="T">IReusable (chainable) type</typeparam>
    /// <returns>
    /// Simple moving average or <see langword="null"/>
    /// if incalculable <see langword="double.NaN"/>
    /// values are in range.
    /// </returns>
    [Category("Public API only")]
    public static double? Average<T>(
        this IReadOnlyList<T> values,
        int lookbackPeriods,
        int? endIndex = null)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(values);

        return Increment(
            values,
            lookbackPeriods,
            endIndex ?? values.Count - 1)
           .NaN2Null();
    }

    /// <summary>
    /// Simple moving average calculation
    /// </summary>
    /// <param name="source">List of chainable values</param>
    /// <param name="lookbackPeriods">Window to evaluate, prior to 'endIndex'</param>
    /// <param name="endIndex">Index position to evaluate.</param>
    /// <typeparam name="T">IReusable (chainable) type</typeparam>
    /// <returns>
    /// Simple moving average or <see langword="double.NaN"/>
    /// when incalculable.
    /// </returns>
    internal static double Increment<T>(
        IReadOnlyList<T> source,
        int lookbackPeriods,
        int endIndex)
        where T : IReusable
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Count)
        {
            return double.NaN;
        }

        double sum = 0;
        for (int i = endIndex - lookbackPeriods + 1; i <= endIndex; i++)
        {
            sum += source[i].Value;
        }

        return sum / lookbackPeriods;

        // TODO: apply this SMA increment method more widely in other indicators (see EMA example)
    }

    [ExcludeFromCodeCoverage]
    [Category("Experimental")]
    internal static double[] Increment(this double[] prices, int period)
    {
        // TODO: remove/consider experiment, has rounding errors

        int count = prices.Length - period + 1;
        double[] sma = new double[count];

        int simdWidth = Vector<double>.Count;
        for (int i = 0; i < count; i++)
        {
            Vector<double> sumVector = Vector<double>.Zero;

            int j;
            for (j = 0; j <= period - simdWidth; j += simdWidth)
            {
                Vector<double> priceVector = new(prices, i + j);
                sumVector += priceVector;
            }

            double sum = 0;
            for (; j < period; j++) // remainder loop
            {
                sum += prices[i + j];
            }
            sum += Vector.Dot(sumVector, Vector<double>.One);

            sma[i] = sum / period;
        }

        return sma;
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }
}
