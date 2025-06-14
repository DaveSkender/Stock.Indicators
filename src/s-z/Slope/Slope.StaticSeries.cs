namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Slope and Linear Regression for a given source list and lookback period.
/// </summary>
public static partial class Slope
{
    /// <summary>
    /// Calculates the Slope and Linear Regression for a given source list and lookback period.
    /// </summary>
    /// <typeparam name="T">The type of the source items, must implement IReusable.</typeparam>
    /// <param name="source">The source list to analyze.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A read-only list of Slope results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback period is less than 1.</exception>
    public static IReadOnlyList<SlopeResult> ToSlope<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<SlopeResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip initialization period
            if (i < lookbackPeriods - 1)
            {
                results.Add(new(s.Timestamp));
                continue;
            }

            // get averages for period
            double sumX = 0;
            double sumY = 0;

            for (int p = i - lookbackPeriods + 1; p <= i; p++)
            {
                T ps = source[p];

                sumX += p + 1d;
                sumY += ps.Value;
            }

            double avgX = sumX / lookbackPeriods;
            double avgY = sumY / lookbackPeriods;

            // least squares method
            double sumSqX = 0;
            double sumSqY = 0;
            double sumSqXy = 0;

            for (int p = i - lookbackPeriods + 1; p <= i; p++)
            {
                T ps = source[p];

                double devX = p + 1d - avgX;
                double devY = ps.Value - avgY;

                sumSqX += devX * devX;
                sumSqY += devY * devY;
                sumSqXy += devX * devY;
            }

            double? slope = (sumSqXy / sumSqX).NaN2Null();
            double? intercept = (avgY - (slope * avgX)).NaN2Null();

            // calculate Standard Deviation and R-Squared
            double stdDevX = Math.Sqrt(sumSqX / lookbackPeriods);
            double stdDevY = Math.Sqrt(sumSqY / lookbackPeriods);

            double? rSquared = null;

            if (stdDevX * stdDevY != 0)
            {
                double arrr = sumSqXy / (stdDevX * stdDevY) / lookbackPeriods;
                rSquared = (arrr * arrr).NaN2Null();
            }

            // write results
            SlopeResult r = new(
                Timestamp: s.Timestamp,
                Slope: slope,
                Intercept: intercept,
                StdDev: stdDevY.NaN2Null(),
                RSquared: rSquared,
                Line: null); // re-written below

            results.Add(r);
        }

        // insufficient length for last line
        if (length < lookbackPeriods)
        {
            return results;
        }

        // add last Line (y = mx + b)
        SlopeResult last = results.Last();

        for (int p = length - lookbackPeriods; p < length; p++)
        {
            SlopeResult d = results[p];

            results[p] = d with {
                Line = (decimal?)((last.Slope * (p + 1)) + last.Intercept).NaN2Null()
            };
        }

        return results;
    }
}
