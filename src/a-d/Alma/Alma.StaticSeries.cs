namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Arnaud Legoux Moving Average (ALMA).
/// </summary>
public static partial class Alma
{
    /// <summary>
    /// Calculates the Arnaud Legoux Moving Average (ALMA) for a series of data.
    /// </summary>
    /// <typeparam name="T">The type of elements in the source series.</typeparam>
    /// <param name="source">The source series.</param>
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 9.</param>
    /// <param name="offset">The offset for the ALMA calculation. Default is 0.85.</param>
    /// <param name="sigma">The sigma for the ALMA calculation. Default is 6.</param>
    /// <returns>A list of ALMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source series is null.</exception>
    public static IReadOnlyList<AlmaResult> ToAlma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods, offset, sigma);

        // initialize
        int length = source.Count;
        List<AlmaResult> results = new(length);

        // determine price weight constants
        double m = offset * (lookbackPeriods - 1);
        double s = lookbackPeriods / sigma;

        double[] weight = new double[lookbackPeriods];
        double norm = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double wt = Math.Exp(-((i - m) * (i - m)) / (2 * s * s));
            weight[i] = wt;
            norm += wt;
        }

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            double alma = double.NaN;

            if (i + 1 >= lookbackPeriods)
            {
                double weightedSum = 0;
                int n = 0;

                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    weightedSum += weight[n] * ps.Value;
                    n++;
                }

                alma = weightedSum / norm;
            }

            results.Add(
            new(Timestamp: source[i].Timestamp,
                Alma: alma.NaN2Null()));
        }

        return results;
    }
}
