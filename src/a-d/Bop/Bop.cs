namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // BALANCE OF POWER
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<BopResult> GetBop<TQuote>(
        this IEnumerable<TQuote> quotes,
        int smoothPeriods = 14)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateBop(smoothPeriods);

        // initialize
        int length = quotesList.Count;
        List<BopResult> results = new(length);

        double?[] raw = quotesList
            .Select(x => (x.High != x.Low) ?
                (double?)((x.Close - x.Open) / (x.High - x.Low)) : null)
            .ToArray();

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            BopResult r = new()
            {
                Date = quotesList[i].Date
            };

            if (i >= smoothPeriods - 1)
            {
                double? sum = 0;
                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sum += raw[p];
                }

                r.Bop = sum / smoothPeriods;
            }

            results.Add(r);
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<BopResult> RemoveWarmupPeriods(
        this IEnumerable<BopResult> results)
    {
        int removePeriods = results
            .ToList()
            .FindIndex(x => x.Bop != null);

        return results.Remove(removePeriods);
    }

    // parameter validation
    private static void ValidateBop(
        int smoothPeriods)
    {
        // check parameter arguments
        if (smoothPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothPeriods), smoothPeriods,
                "Smoothing periods must be greater than 0 for BOP.");
        }
    }
}
