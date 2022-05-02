namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ToQuotes(
        this IEnumerable<DpoResult> results)
    {
        return results
          .Where(x => x.Dpo != null)
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = x.Dpo,
              High = x.Dpo,
              Low = x.Dpo,
              Close = x.Dpo,
              Volume = x.Dpo
          })
          .ToList();
    }
}
