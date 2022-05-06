namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ToQuotes(
        this IEnumerable<AdlResult> results) => results
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = (decimal?)x.Adl,
              High = (decimal?)x.Adl,
              Low = (decimal?)x.Adl,
              Close = (decimal?)x.Adl,
              Volume = (decimal?)x.Adl
          })
          .ToList();
}
