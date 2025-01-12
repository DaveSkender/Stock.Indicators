/// <summary>
/// Provides utility methods for the VWAP (Volume Weighted Average Price) indicator.
/// </summary>
public static partial class Vwap
{
    // remove recommended periods
    /// <summary>
    /// Removes the warmup periods from the VWAP results.
    /// </summary>
    /// <param name="results">The list of VWAP results.</param>
    /// <returns>A list of VWAP results with the warmup periods removed.</returns>
    /// <inheritdoc cref="Reusable.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<VwapResult> RemoveWarmupPeriods(
        this IReadOnlyList<VwapResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Vwap != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    /// <summary>
    /// Validates the parameters for the VWAP calculation.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="startDate">The optional start date for the VWAP calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the start date is earlier than the first quote's timestamp.
    /// </exception>
    internal static void Validate(
        IReadOnlyList<QuoteD> quotes,
        DateTime? startDate)
    {
        // nothing to do for 0 length
        if (quotes.Count == 0)
        {
            return;
        }

        // check parameter arguments (intentionally after quotes check)
        if (startDate < quotes[0].Timestamp)
        {
            throw new ArgumentOutOfRangeException(nameof(startDate), startDate,
                "Start Timestamp must be within the quotes range for VWAP.");
        }
    }
}
