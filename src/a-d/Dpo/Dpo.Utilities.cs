namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<DpoResult> results)
    {
        return results
          .Where(x => x.Dpo != null)
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = (decimal)x.Dpo,
              High = (decimal)x.Dpo,
              Low = (decimal)x.Dpo,
              Close = (decimal)x.Dpo,
              Volume = (decimal)x.Dpo
          })
          .ToList();
    }
}
