namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ENDPOINT MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<EpmaResult> GetEpma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateEpma(lookbackPeriods);

        // initialize
        List<SlopeResult> slopeResults = GetSlope(quotes, lookbackPeriods)
            .ToList();

        int length = slopeResults.Count;
        List<EpmaResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            SlopeResult s = slopeResults[i];

            EpmaResult r = new()
            {
                Date = s.Date,
                Epma = (decimal?)((s.Slope * (i + 1)) + s.Intercept)
            };

            results.Add(r);
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<EpmaResult> RemoveWarmupPeriods(
        this IEnumerable<EpmaResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.Epma != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    private static void ValidateEpma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Epma.");
        }
    }
}
