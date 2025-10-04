namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Endpoint Moving Average (EPMA) calculations.
/// </summary>
public static partial class Epma
{
    /// <summary>
    /// Removes the recommended warmup periods from the EPMA results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<EpmaResult> RemoveWarmupPeriods(
        this IReadOnlyList<EpmaResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
          .FindIndex(x => x.Epma != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the lookback periods for EPMA calculations.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 0.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Epma.");
        }
    }

    /// <summary>
    /// Calculates EPMA increment for the current position using linear regression.
    /// </summary>
    /// <typeparam name="T">The type of the source items, must implement IReusable.</typeparam>
    /// <param name="source">The source data provider cache.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="endIndex">The current index position to evaluate.</param>
    /// <returns>The EPMA value or double.NaN if incalculable.</returns>
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

        // Calculate linear regression for the lookback window
        int startIndex = endIndex - lookbackPeriods + 1;

        // Calculate averages using global position indices
        double sumX = 0;
        double sumY = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            sumX += startIndex + i + 1d; // X values are global positions (1-based)
            sumY += source[startIndex + i].Value;
        }

        double avgX = sumX / lookbackPeriods;
        double avgY = sumY / lookbackPeriods;

        // Least squares method
        double sumSqX = 0;
        double sumSqXy = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double devX = (startIndex + i + 1d) - avgX;
            double devY = source[startIndex + i].Value - avgY;

            sumSqX += devX * devX;
            sumSqXy += devX * devY;
        }

        if (sumSqX == 0)
        {
            return double.NaN;
        }

        double slope = sumSqXy / sumSqX;
        double intercept = avgY - (slope * avgX);

        // EPMA calculation: slope * (endpoint_index + 1) + intercept
        // The endpoint index is the actual position (endIndex) in the dataset (1-based)
        double epma = (slope * (endIndex + 1)) + intercept;

        return epma;
    }
}
