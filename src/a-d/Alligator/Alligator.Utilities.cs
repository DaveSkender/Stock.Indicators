namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Williams Alligator indicator.
/// </summary>
public static partial class Alligator
{
    /// <summary>
    /// Converts Alligator results to a chainable list using the specified line.
    /// </summary>
    /// <param name="results">The list of Alligator results.</param>
    /// <param name="line">The Alligator line to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<AlligatorResult> results,
        AlligatorLine line = AlligatorLine.Jaw)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            AlligatorResult r = results[i];

            double value = line switch {
                AlligatorLine.Jaw => r.Jaw.Null2NaN(),
                AlligatorLine.Teeth => r.Teeth.Null2NaN(),
                AlligatorLine.Lips => r.Lips.Null2NaN(),
                _ => throw new ArgumentOutOfRangeException(nameof(line), line, "Invalid line provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes non-essential records containing null values for Jaw, Teeth, and Lips.
    /// </summary>
    /// <param name="results">The Alligator results to evaluate.</param>
    /// <returns>A condensed list of Alligator results.</returns>
    public static IReadOnlyList<AlligatorResult> Condense(
        this IEnumerable<AlligatorResult> results)
    {
        List<AlligatorResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.Jaw is null && x.Teeth is null && x.Lips is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Removes the recommended quantity of results from the beginning of the results list.
    /// </summary>
    /// <param name="results">The Alligator results to evaluate.</param>
    /// <returns>A pruned list of Alligator results.</returns>
    public static IReadOnlyList<AlligatorResult> RemoveWarmupPeriods(
        this IReadOnlyList<AlligatorResult> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        int removePeriods = results
          .FindIndex(static x => x.Jaw != null) + 251;

        return results.Remove(removePeriods);
    }

    /// <summary>
    /// Validates the parameters for the Williams Alligator indicator.
    /// </summary>
    /// <param name="jawPeriods">The lookback periods for the Jaw.</param>
    /// <param name="jawOffset">The offset periods for the Jaw.</param>
    /// <param name="teethPeriods">The lookback periods for the Teeth.</param>
    /// <param name="teethOffset">The offset periods for the Teeth.</param>
    /// <param name="lipsPeriods">The lookback periods for the Lips.</param>
    /// <param name="lipsOffset">The offset periods for the Lips.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any parameter is out of range.</exception>
    internal static void Validate(
        int jawPeriods,
        int jawOffset,
        int teethPeriods,
        int teethOffset,
        int lipsPeriods,
        int lipsOffset)
    {
        // check parameter arguments
        if (jawPeriods <= teethPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                "Jaw lookback periods must be greater than Teeth lookback periods for Alligator.");
        }

        if (teethPeriods <= lipsPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                "Teeth lookback periods must be greater than Lips lookback periods for Alligator.");
        }

        if (lipsPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lipsPeriods), lipsPeriods,
                "Lips lookback periods must be greater than 0 for Alligator.");
        }

        if (jawOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(jawOffset), jawOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (teethOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(teethOffset), teethOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (lipsOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lipsOffset), lipsOffset,
                "Jaw offset periods must be greater than 0 for Alligator.");
        }

        if (jawPeriods + jawOffset <= teethPeriods + teethOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(jawPeriods), jawPeriods,
                "Jaw lookback + offset are too small for Alligator.");
        }

        if (teethPeriods + teethOffset <= lipsPeriods + lipsOffset)
        {
            throw new ArgumentOutOfRangeException(nameof(teethPeriods), teethPeriods,
                "Teeth lookback + offset are too small for Alligator.");
        }
    }
}
