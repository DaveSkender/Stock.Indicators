namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // CONDENSE (REMOVE null results)
    /// <inheritdoc cref="ReusableUtility.Condense{T}(IEnumerable{T})"/>
    public static IEnumerable<MaEnvelopeResult> Condense(
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
