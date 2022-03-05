namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<RsiResult> results)
    {
        return results
          .Where(x => x.Rsi != null)
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = (decimal)x.Rsi,
              High = (decimal)x.Rsi,
              Low = (decimal)x.Rsi,
              Close = (decimal)x.Rsi,
              Volume = (decimal)x.Rsi
          })
          .ToList();
    }

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<RsiResult> RemoveWarmupPeriods(
        this IEnumerable<RsiResult> results)
    {
        int n = results
            .ToList()
            .FindIndex(x => x.Rsi != null);

        return results.Remove(10 * n);
    }
}
