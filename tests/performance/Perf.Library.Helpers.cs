namespace Tests.Performance;

// HELPERS, both public and private

public class LibraryHelpers
{
    private static IEnumerable<Quote> _h;
    private static IEnumerable<Quote> _i;

    [GlobalSetup]
    public static void Setup() => _h = TestData.GetDefault();

    [GlobalSetup(Targets = [nameof(Aggregate)])]
    public static void SetupIntraday() => _i = TestData.GetIntraday();

    [Benchmark]
    public object ToSortedList() => _h.ToSortedList();

    [Benchmark]
    public object ToSortedCollection() => _h.ToSortedCollection();

    [Benchmark]
    public object ToListQuoteD() => _h.ToQuoteD();

    [Benchmark]
    public object ToReusableClose() => _h.ToReusableList(CandlePart.Close);

    [Benchmark]
    public object ToReusableOhlc4() => _h.ToReusableList(CandlePart.Ohlc4);

    [Benchmark]
    public object ToCandleResults() => _h.ToCandleResults();

    [Benchmark]
    public object Validate() => _h.Validate();

    [Benchmark]
    public object Aggregate() => _i.Aggregate(PeriodSize.FifteenMinutes);
}
