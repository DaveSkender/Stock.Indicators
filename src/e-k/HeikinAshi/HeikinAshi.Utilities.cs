namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // convert results to quotes
    public static IEnumerable<Quote> ToQuotes(
    this IEnumerable<HeikinAshiResult> results)
        => results.Cast<Quote>();
}
