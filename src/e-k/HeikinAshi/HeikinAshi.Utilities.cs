namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert results to quotes
    public static IEnumerable<Quote> ToQuotes(
    this IEnumerable<HeikinAshiResult> results)
      => results
        .Select(x => new Quote
        {
            Timestamp = x.Timestamp,
            Open = x.Open,
            High = x.High,
            Low = x.Low,
            Close = x.Close,
            Volume = x.Volume
        })
        .OrderBy(x => x.Timestamp)
        .ToList();
}
