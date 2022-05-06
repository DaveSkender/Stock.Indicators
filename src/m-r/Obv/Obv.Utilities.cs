namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ToQuotes(
        this IEnumerable<ObvResult> results) => results
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = (decimal?)x.Obv,
              High = (decimal?)x.Obv,
              Low = (decimal?)x.Obv,
              Close = (decimal?)x.Obv,
              Volume = (decimal?)x.Obv
          })
          .ToList();
}
