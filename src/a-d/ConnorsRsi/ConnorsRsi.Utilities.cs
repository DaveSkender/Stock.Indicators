namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IEnumerable<ConnorsRsiResult> RemoveWarmupPeriods(
        this IEnumerable<ConnorsRsiResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.ConnorsRsi != null);

        return results.Remove(n);
    }
}
