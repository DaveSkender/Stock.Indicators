/// <summary>
/// Provides utility methods for Ichimoku Cloud calculations.
/// </summary>
public static partial class Ichimoku
{
    /// <summary>
    /// Removes empty (null) periods from the Ichimoku Cloud results.
    /// </summary>
    /// <param name="results">The list of Ichimoku Cloud results to condense.</param>
    /// <returns>A condensed list of Ichimoku Cloud results without null periods.</returns>
    public static IReadOnlyList<IchimokuResult> Condense(
        this IReadOnlyList<IchimokuResult> results)
    {
        List<IchimokuResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match: x =>
                   x.TenkanSen is null
                && x.KijunSen is null
                && x.SenkouSpanA is null
                && x.SenkouSpanB is null
                && x.ChikouSpan is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Validates the parameters for the Ichimoku Cloud calculation.
    /// </summary>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="senkouOffset">The number of periods for the Senkou offset.</param>
    /// <param name="chikouOffset">The number of periods for the Chikou offset.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when any of the parameters are out of their valid range.
    /// </exception>
    internal static void Validate(
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
    {
        if (tenkanPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tenkanPeriods), tenkanPeriods,
                "Tenkan periods must be greater than 0 for Ichimoku Cloud.");
        }

        if (kijunPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kijunPeriods), kijunPeriods,
                "Kijun periods must be greater than 0 for Ichimoku Cloud.");
        }

        if (senkouBPeriods <= kijunPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(senkouBPeriods), senkouBPeriods,
                "Senkou B periods must be greater than Kijun periods for Ichimoku Cloud.");
        }

        if (senkouOffset < 0 || chikouOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(senkouOffset), senkouOffset,
                "Senkou and Chikou offset periods must be non-negative for Ichimoku Cloud.");
        }
    }
}
