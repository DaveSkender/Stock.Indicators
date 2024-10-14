namespace Skender.Stock.Indicators;

public static partial class Correlation
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<CorrResult> RemoveWarmupPeriods(
        this IReadOnlyList<CorrResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Correlation != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate<T>(
        List<T> sourceA,
        List<T> sourceB,
        int lookbackPeriods)
        where T : ISeries
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Correlation.");
        }

        // check quotes
        if (sourceA.Count != sourceB.Count)
        {
            throw new InvalidQuotesException(
                nameof(sourceB),
                "B quotes should have at least as many records as A quotes for Correlation.");
        }
    }
}
