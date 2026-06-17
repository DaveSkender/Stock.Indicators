namespace Performance;

// INTERNAL UTILITIES

[ShortRunJob]
public class Utility
{
    private static readonly IReadOnlyList<Bar> bars = Data.GetDefault();
    private static readonly IReadOnlyList<Bar> intraday = Data.GetIntraday();
    private const int n = 14;

    private static readonly Bar bar = bars[0];

    [Benchmark]
    public object ToSortedList() => bars.ToSortedList();

    [Benchmark]
    public object ToListBarD() => bars.ToBarDList();

    [Benchmark]
    public object ToReusableClose() => bars.ToReusable(CandlePart.Close);

    [Benchmark]
    public object ToReusableOhlc4() => bars.ToReusable(CandlePart.OHLC4);

    [Benchmark]
    public object ToCandleResults() => bars.ToCandles();

    [Benchmark]
    public object ToStringOutType() => bar.ToStringOut();

    [Benchmark]
    public object ToStringOutList() => bars.ToStringOut();

    [Benchmark]
    public object Validate() => bars.Validate();

    [Benchmark]
    public object Aggregate() => intraday.Aggregate(BarInterval.FifteenMinutes);

    [Benchmark]
    public object RemoveWarmupPeriods()
    {
        IReadOnlyList<RsiResult> results = bars.ToRsi(n);
        return results.RemoveWarmupPeriods();
    }
}
