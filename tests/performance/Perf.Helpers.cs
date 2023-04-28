using BenchmarkDotNet.Attributes;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Performance;

// HELPERS, both public and private

public class HelperPerformance
{
    private static IEnumerable<Quote> h;
    private static IEnumerable<Quote> i;

    [GlobalSetup]
    public static void Setup() => h = TestData.GetDefault();

    [GlobalSetup(Targets = new[] { nameof(Aggregate) })]
    public static void SetupIntraday() => i = TestData.GetIntraday();

    [Benchmark]
    public object ToSortedList() => h.ToSortedList();

    [Benchmark]
    public object ToSortedCollection() => h.ToSortedCollection();

    [Benchmark]
    public object ToListQuoteD() => h.ToQuoteD();

    [Benchmark]
    public object Validate() => h.Validate();

    [Benchmark]
    public object Aggregate() => i.Aggregate(PeriodSize.FifteenMinutes);

    [Benchmark]
    public object ToTuple() => h.ToTuple(CandlePart.Close);

    [Benchmark]
    public object ToCandleResults() => h.ToCandleResults();
}
