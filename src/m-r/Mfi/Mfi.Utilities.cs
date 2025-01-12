/// <summary>
/// Provides utility methods for the Money Flow Index (MFI).
/// </summary>
public static partial class Mfi
{
    /// <summary>
    /// Removes the recommended warmup periods from the MFI results.
    /// </summary>
    /// <param name="results">The list of MFI results.</param>
    /// <returns>A list of MFI results with the warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<MfiResult> RemoveWarmupPeriods(
        this IReadOnlyList<MfiResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Mfi != null);

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the parameters for the MFI calculation.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to use for the MFI calculation.</param>
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
                "Lookback periods must be greater than 1 for MFI.");
        }
    }
}
