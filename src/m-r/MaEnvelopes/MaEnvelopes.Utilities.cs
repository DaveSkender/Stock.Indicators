namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the Moving Average Envelopes.
/// </summary>
public static partial class MaEnvelopes
{
    /// <summary>
    /// Converts Moving Average Envelope results to a chainable list using the specified field.
    /// </summary>
    /// <param name="results">The list of Moving Average Envelope results.</param>
    /// <param name="field">The field to use for chaining.</param>
    /// <returns>A list of chainable values.</returns>
    public static IReadOnlyList<QuotePart> Use(
        this IReadOnlyList<MaEnvelopeResult> results,
        MaEnvelopeField field = MaEnvelopeField.Centerline)
    {
        ArgumentNullException.ThrowIfNull(results);
        int length = results.Count;
        List<QuotePart> list = new(length);

        for (int i = 0; i < length; i++)
        {
            MaEnvelopeResult r = results[i];

            double value = field switch {
                MaEnvelopeField.Centerline => r.Centerline.Null2NaN(),
                MaEnvelopeField.UpperEnvelope => r.UpperEnvelope.Null2NaN(),
                MaEnvelopeField.LowerEnvelope => r.LowerEnvelope.Null2NaN(),
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Invalid field provided.")
            };

            list.Add(new QuotePart(r.Timestamp, value));
        }

        return list;
    }

    /// <summary>
    /// Removes empty (null) periods from the Moving Average Envelope results.
    /// </summary>
    /// <param name="results">The list of Moving Average Envelope results.</param>
    /// <returns>A list of Moving Average Envelope results with empty periods removed.</returns>
    public static IReadOnlyList<MaEnvelopeResult> Condense(
        this IReadOnlyList<MaEnvelopeResult> results)
    {
        List<MaEnvelopeResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                static x => x.UpperEnvelope is null && x.LowerEnvelope is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    /// <summary>
    /// Validates the parameters for the Moving Average Envelopes calculation.
    /// </summary>
    /// <param name="percentOffset">The percentage offset for the envelopes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the percent offset is less than or equal to 0.</exception>
    internal static void Validate(
        double percentOffset)
    {
        // check parameter arguments
        if (percentOffset <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentOffset), percentOffset,
                "Percent Offset must be greater than 0 for Moving Average Envelopes.");
        }
    }
}
