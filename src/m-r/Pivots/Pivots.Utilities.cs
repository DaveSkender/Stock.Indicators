namespace Skender.Stock.Indicators;

// PIVOTS (UTILITIES)

public static partial class Pivots
{
    // remove empty (null) periods
    /// <inheritdoc cref="Reusable.Condense{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<PivotsResult> Condense(
        this IReadOnlyList<PivotsResult> results)
    {
        List<PivotsResult> resultsList = results
            .ToList();

        resultsList
            .RemoveAll(match:
                x => x.HighPoint is null && x.LowPoint is null);

        return resultsList.ToSortedList();
    }

    // parameter validation
    internal static void Validate(
        int leftSpan,
        int rightSpan,
        int maxTrendPeriods,
        string caller = "Pivots")
    {
        // check parameter arguments
        if (rightSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(rightSpan), rightSpan,
                $"Right span must be at least 2 for {caller}.");
        }

        if (leftSpan < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(leftSpan), leftSpan,
                $"Left span must be at least 2 for {caller}.");
        }

        if (maxTrendPeriods <= leftSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(leftSpan), leftSpan,
                $"Lookback periods must be greater than the Left window span for {caller}.");
        }
    }
}
