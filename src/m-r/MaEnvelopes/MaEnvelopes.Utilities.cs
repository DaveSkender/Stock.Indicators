namespace Skender.Stock.Indicators;

// MOVING AVERAGE ENVELOPES (UTILITIES)

public static partial class MaEnvelopes
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<MaEnvelopeResult> Condense(
        this IReadOnlyList<MaEnvelopeResult> results)
    {
        List<MaEnvelopeResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperEnvelope is null && x.LowerEnvelope is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }

    // parameter validation
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
