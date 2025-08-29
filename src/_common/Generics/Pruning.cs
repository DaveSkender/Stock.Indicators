namespace Skender.Stock.Indicators;

/// <summary>
/// Utility for removing and pruning series data.
/// </summary>
public static class Pruning
{
    /// <summary>
    /// Removes a specified number of warmup periods from the beginning of the series.
    /// </summary>
    /// <typeparam name="T">The type of elements in the series.</typeparam>
    /// <param name="series">The series from which to remove warmup periods.</param>
    /// <param name="removePeriods">The number of periods to remove.</param>
    /// <returns>A new series with the specified number of warmup periods removed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when removePeriods is less than 0.</exception>
    public static IReadOnlyList<T> RemoveWarmupPeriods<T>(
        this IReadOnlyList<T> series,
        int removePeriods)
        => removePeriods < 0
            ? throw new ArgumentOutOfRangeException(nameof(removePeriods), removePeriods,
                "If specified, the Remove Periods value must be greater than or equal to 0.")
            : series.Remove(removePeriods);

    /// <summary>
    /// Removes a specified number of periods from the beginning of the series.
    /// </summary>
    /// <typeparam name="T">The type of elements in the series.</typeparam>
    /// <param name="series">The series from which to remove periods.</param>
    /// <param name="removePeriods">The number of periods to remove.</param>
    /// <returns>A new list with the specified number of periods removed.</returns>
    internal static List<T> Remove<T>(
        this IReadOnlyList<T> series,
        int removePeriods)
    {
        List<T> seriesList = series.ToList();

        if (seriesList.Count <= removePeriods)
        {
            return [];
        }

        if (removePeriods <= 0)
        {
            return seriesList;
        }

        for (int i = 0; i < removePeriods; i++)
        {
            seriesList.RemoveAt(0);
        }

        return seriesList;
    }
}
