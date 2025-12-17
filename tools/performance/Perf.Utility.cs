namespace Performance;

// INTERNAL UTILITIES

[ShortRunJob]
public class Utility
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> intraday = Data.GetIntraday();
    private static readonly Quote quote = quotes[0];

    [Benchmark]
    public object ToSortedList() => quotes.ToSortedList();

    [Benchmark]
    public object ToListQuoteD() => quotes.ToQuoteDList();

    [Benchmark]
    public object ToListQuoteX() => quotes.ToQuoteXList();

    [Benchmark]
    public object ToReusableClose() => quotes.ToReusable(CandlePart.Close);

    [Benchmark]
    public object ToReusableOhlc4() => quotes.ToReusable(CandlePart.OHLC4);

    [Benchmark]
    public object ToCandleResults() => quotes.ToCandles();

    [Benchmark]
    public object ToStringOutType() => quote.ToStringOut();

    [Benchmark]
    public object ToStringOutList() => quotes.ToStringOut();

    [Benchmark]
    public object Validate() => quotes.Validate();

    [Benchmark]
    public object Aggregate() => intraday.Aggregate(PeriodSize.FifteenMinutes);

    [Benchmark]
    public object RemoveWarmupPeriods()
    {
        IReadOnlyList<RsiResult> results = quotes.ToRsi(14);
        return results.RemoveWarmupPeriods();
    }
}
