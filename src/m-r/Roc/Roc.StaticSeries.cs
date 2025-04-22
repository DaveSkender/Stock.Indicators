namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Rate of Change (ROC) series.
/// </summary>
public static partial class Roc
{
    /// <summary>
    /// Converts a list of reusable values to a list of ROC results.
    /// </summary>
    /// <typeparam name="T">The type of the reusable values.</typeparam>
    /// <param name="source">The list of reusable values.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the ROC calculation.</param>
    /// <returns>A list of ROC results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    [Series("ROC", "Rate of Change", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<RocResult> ToRoc<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<RocResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            double roc;
            double momentum;

            if (i + 1 > lookbackPeriods)
            {
                T back = source[i - lookbackPeriods];

                momentum = s.Value - back.Value;

                roc = back.Value == 0
                    ? double.NaN
                    : 100d * momentum / back.Value;
            }
            else
            {
                momentum = double.NaN;
                roc = double.NaN;
            }

            RocResult r = new(
                Timestamp: s.Timestamp,
                Momentum: momentum.NaN2Null(),
                Roc: roc.NaN2Null());

            results.Add(r);
        }

        return results;
    }
}
