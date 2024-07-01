namespace Skender.Stock.Indicators;

public static partial class Sma
{
    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SmaResult> RemoveWarmupPeriods(
        this IEnumerable<SmaResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sma != null);

        return results.Remove(removePeriods);
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<SmaAnalysis> RemoveWarmupPeriods(
        this IEnumerable<SmaAnalysis> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Sma != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }
}
