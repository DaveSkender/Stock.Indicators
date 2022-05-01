namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ToQuotes(
        this IEnumerable<HurstResult> results)
        => results
          .Where(x => x.HurstExponent != null)
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = (decimal)x.HurstExponent,
              High = (decimal)x.HurstExponent,
              Low = (decimal)x.HurstExponent,
              Close = (decimal)x.HurstExponent
          })
          .ToList();

    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<HurstResult> RemoveWarmupPeriods(
        this IEnumerable<HurstResult> results)
    {
        int removePeriods = results
          .ToList()
          .FindIndex(x => x.HurstExponent != null);

        return results.Remove(removePeriods);
    }
}
