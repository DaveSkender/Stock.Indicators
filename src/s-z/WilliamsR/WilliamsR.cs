namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // WILLIAM %R OSCILLATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<WilliamsResult> GetWilliamsR<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateWilliam(lookbackPeriods);

        // convert Stochastic to William %R
        return GetStoch(quotes, lookbackPeriods, 1, 1) // fast variant
            .Select(s => new WilliamsResult
            {
                Date = s.Date,
                WilliamsR = s.Oscillator - 100
            })
            .ToList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<WilliamsResult> RemoveWarmupPeriods(
        this IEnumerable<WilliamsResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.WilliamsR != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    private static void ValidateWilliam(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for William %R.");
        }
    }
}
