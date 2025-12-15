namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // remove recommended periods
    // <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    //
    // public static IEnumerable<VpvrResult> RemoveWarmupPeriods(
    //    this IEnumerable<VpvrResult> results)
    // {
    //    int removePeriods = results
    //        .ToList()
    //        .FindIndex(x => x.Volume != null);
    //
    //    return results.Remove(removePeriods);
    // }
}
