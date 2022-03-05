namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ChaikinOscResult> RemoveWarmupPeriods(
        this IEnumerable<ChaikinOscResult> results)
    {
        int s = results
            .ToList()
            .FindIndex(x => x.Oscillator != null) + 1;

        return results.Remove(s + 100);
    }
}
