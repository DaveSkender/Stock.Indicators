namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <inheritdoc cref="Utility.RemoveWarmupPeriods{T}(IReadOnlyList{T})"/>
    public static IReadOnlyList<MamaResult> RemoveWarmupPeriods(
        this IReadOnlyList<MamaResult> results) => results.Remove(50);
}
