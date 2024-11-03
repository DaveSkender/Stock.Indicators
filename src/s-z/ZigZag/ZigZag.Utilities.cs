namespace Skender.Stock.Indicators;

// ZIG ZAG (UTILITIES)

public static partial class ZigZag
{
    // remove empty (null) periods
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ZigZagResult> Condense(
        this IReadOnlyList<ZigZagResult> results)
    {
        List<ZigZagResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.PointType is null);

        return resultsList.ToSortedList();
    }

    // parameter validation
    internal static void Validate(
        decimal percentChange)
    {
        // check parameter arguments
        if (percentChange <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentChange), percentChange,
                "Percent change must be greater than 0 for ZIGZAG.");
        }
    }
}
