namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<StochResult> RemoveWarmupPeriods(
        this IEnumerable<StochResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Oscillator != null);

        return results.Remove(removePeriods);
    }
}
