namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // DOUBLE EXPONENTIAL MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<DemaResult> GetDoubleEma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // check parameter arguments
        ValidateDema(lookbackPeriods);

        // initialize
        List<DemaResult> results = new(bdList.Count);
        List<EmaResult> emaN = CalcEma(bdList, lookbackPeriods);

        List<BasicD> bd2 = emaN
            .Where(x => x.Ema != null)
            .Select(x => new BasicD { Date = x.Date, Value = (double)x.Ema })
            .ToList();  // note: ToList seems to be required when changing data

        List<EmaResult> emaN2 = CalcEma(bd2, lookbackPeriods);

        // compose final results
        for (int i = 0; i < emaN.Count; i++)
        {
            EmaResult e1 = emaN[i];
            int index = i + 1;

            DemaResult result = new()
            {
                Date = e1.Date
            };

            if (index >= (2 * lookbackPeriods) - 1)
            {
                EmaResult e2 = emaN2[index - lookbackPeriods];
                result.Dema = (2 * e1.Ema) - e2.Ema;
            }

            results.Add(result);
        }

        return results;
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<DemaResult> RemoveWarmupPeriods(
        this IEnumerable<DemaResult> results)
    {
        int n2 = results
          .ToList()
          .FindIndex(x => x.Dema != null) + 2;

        return results.Remove(n2 + 100);
    }

    // parameter validation
    private static void ValidateDema(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for DEMA.");
        }
    }
}
