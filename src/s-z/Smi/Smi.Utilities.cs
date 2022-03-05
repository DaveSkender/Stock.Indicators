namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SmiResult> RemoveWarmupPeriods(
        this IEnumerable<SmiResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Smi != null);

        return results.Remove(removePeriods + 2 + 100);
    }
}
