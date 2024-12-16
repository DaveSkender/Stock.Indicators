namespace Performance;

// INTERNAL UTILITIES

[ShortRunJob]
public class Utility
{
    private static readonly IReadOnlyList<Quote> q = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> i = Data.GetIntraday();

    [Benchmark]
    public object ToSortedList() => q.ToSortedList();

    [Benchmark]
    public object ToListQuoteD() => q.ToQuoteDList();

    [Benchmark]
    public object ToReusableClose() => q.ToReusable(CandlePart.Close);

    [Benchmark]
    public object ToReusableOhlc4() => q.ToReusable(CandlePart.OHLC4);

    [Benchmark]
    public object ToCandleResults() => q.ToCandles();

    [Benchmark]
    public object Validate() => q.Validate();

    [Benchmark]
    public object Aggregate() => i.Aggregate(PeriodSize.FifteenMinutes);
}
