namespace Skender.Stock.Indicators;

/// <summary>
/// Provides utility methods for the ZigZag indicator.
/// </summary>
public static partial class ZigZag
{
    /// <summary>
    /// Removes empty (null) periods from the ZigZag results.
    /// </summary>
    /// <param name="results">The list of ZigZag results.</param>
    /// <returns>A condensed list of ZigZag results without null periods.</returns>
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

    /// <summary>
    /// Validates the parameters for the ZigZag indicator.
    /// </summary>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the percent change is less than or equal to 0.</exception>
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
