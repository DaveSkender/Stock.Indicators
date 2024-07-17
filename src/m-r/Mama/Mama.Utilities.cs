namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="ReusableUtility.RemoveWarmupPeriods{T}(IEnumerable{T})"/>
    public static IReadOnlyList<MamaResult> RemoveWarmupPeriods(
        this IEnumerable<MamaResult> results) => results.Remove(50);
}
