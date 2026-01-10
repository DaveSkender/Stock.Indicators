namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for Ichimoku Cloud calculations.
/// </summary>
public static partial class Ichimoku
{
    /// <summary>
    /// Converts Ichimoku Cloud results to a chainable list using the specified line.
    /// </summary>
    /// <param name="results">The list of Ichimoku Cloud results.</param>
    /// <param name="line">The Ichimoku line to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<IchimokuResult> results,
        IchimokuLine line = IchimokuLine.TenkanSen)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            IchimokuResult r = results[i];

            double value = line switch {
                IchimokuLine.TenkanSen => (double?)r.TenkanSen ?? double.NaN,
                IchimokuLine.KijunSen => (double?)r.KijunSen ?? double.NaN,
                IchimokuLine.SenkouSpanA => (double?)r.SenkouSpanA ?? double.NaN,
                IchimokuLine.SenkouSpanB => (double?)r.SenkouSpanB ?? double.NaN,
                IchimokuLine.ChikouSpan => (double?)r.ChikouSpan ?? double.NaN,
                _ => throw new ArgumentOutOfRangeException(nameof(line), line, "Invalid line provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

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
            .RemoveAll(match: static x =>
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
