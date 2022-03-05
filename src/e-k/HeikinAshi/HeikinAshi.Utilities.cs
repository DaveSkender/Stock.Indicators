namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // convert to quotes
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Convert"]/*' />
    ///
    public static IEnumerable<Quote> ConvertToQuotes(
        this IEnumerable<HeikinAshiResult> results)
    {
        return results
          .Select(x => new Quote
          {
              Date = x.Date,
              Open = x.Open,
              High = x.High,
              Low = x.Low,
              Close = x.Close,
              Volume = x.Volume
          })
          .ToList();
    }
}
