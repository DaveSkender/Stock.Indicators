namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<ConnorsRsiResult> RemoveWarmupPeriods(
        this IReadOnlyList<ConnorsRsiResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.ConnorsRsi != null);

        return results.Remove(n);
    }
}
