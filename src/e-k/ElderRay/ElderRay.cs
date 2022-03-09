namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ELDER-RAY
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
