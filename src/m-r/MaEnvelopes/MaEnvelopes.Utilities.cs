namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="Utility.Condense{T}(IEnumerable{T})"/>
    public static IReadOnlyList<MaEnvelopeResult> Condense(
        this IEnumerable<MaEnvelopeResult> results)
    {
        List<MaEnvelopeResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.UpperEnvelope is null && x.LowerEnvelope is null && x.Centerline is null);

        return resultsList.ToSortedList();
    }
}
