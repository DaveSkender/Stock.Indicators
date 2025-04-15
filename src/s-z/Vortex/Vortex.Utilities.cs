namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Vortex indicator.
/// </summary>
public static partial class Vortex
{
    // remove empty (null) periods
    /// <summary>
    /// Condenses the Vortex results by removing periods with null values.
    /// </summary>
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VortexResult> Condense(
        this IReadOnlyList<VortexResult> results)
    {
        List<VortexResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.Pvi is null && x.Nvi is null);

        return resultsList.ToSortedList();
    }

    // remove recommended periods
    /// <summary>
    /// Removes the warmup periods from the Vortex results.
    /// </summary>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VortexResult> RemoveWarmupPeriods(
        this IReadOnlyList<VortexResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Pvi != null || x.Nvi != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    /// <summary>
    /// Validates the parameters for the Vortex calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the lookback periods are less than or equal to 1.
    /// </exception>
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for VI.");
        }
    }
}
