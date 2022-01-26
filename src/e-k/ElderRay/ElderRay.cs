namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ElderRay
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<ElderRayResult> GetElderRay<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 13)
        where TQuote : IQuote
    {
        // sort quotes
        List<TQuote> quotesList = quotes.SortToList();

        // check parameter arguments
        ValidateElderRay(lookbackPeriods);

        // initialize with EMA
        List<ElderRayResult> results = GetEma(quotes, lookbackPeriods)
            .Select(x => new ElderRayResult
            {
                Date = x.Date,
                Ema = x.Ema
            })
            .ToList();

        // roll through quotes
        for (int i = lookbackPeriods - 1; i < quotesList.Count; i++)
        {
            TQuote q = quotesList[i];
            ElderRayResult r = results[i];

            r.BullPower = q.High - r.Ema;
            r.BearPower = q.Low - r.Ema;
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ElderRayResult> RemoveWarmupPeriods(
        this IEnumerable<ElderRayResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.BullPower != null) + 1;

        return results.Remove(n + 100);
    }

    // parameter validation
    private static void ValidateElderRay(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Elder-ray Index.");
        }
    }
}
