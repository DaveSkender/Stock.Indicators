namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<ConnorsRsiResult> RemoveWarmupPeriods(
        this IEnumerable<ConnorsRsiResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.ConnorsRsi != null);

        return results.Remove(n);
    }
}
