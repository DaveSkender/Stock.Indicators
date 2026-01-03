namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert results to quotes
    public static IEnumerable<Quote> ToQuotes(
    this IEnumerable<HeikinAshiResult> results)
      => results
        .Select(static x => new Quote {
            Date = x.Date,
            Open = x.Open,
            High = x.High,
            Low = x.Low,
            Close = x.Close,
            Volume = x.Volume
        })
        .OrderBy(static x => x.Date)
        .ToList();
}
