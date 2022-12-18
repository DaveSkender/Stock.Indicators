using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance;

// HELPERS, both public and private

[MarkdownExporterAttribute.GitHub]
public class HelperPerformance
{
    private static IEnumerable<Quote> h;
    private static IEnumerable<Quote> i;

    [GlobalSetup]
    public void Setup() => h = TestData.GetDefault();

    [Benchmark]
    public object SortToList() => h.ToSortedCollection();

    [Benchmark]
    public object ToListQuoteD() => h.ToQuoteD();

    [Benchmark]
    public object Validate() => h.Validate();

    [GlobalSetup(Targets = new[] { nameof(Aggregate) })]
    public void SetupIntraday() => i = TestData.GetIntraday();

    [Benchmark]
    public object Aggregate() => i.Aggregate(PeriodSize.FifteenMinutes);

    [Benchmark]
    public object ToTuple() => h.ToTuple(CandlePart.Close);

    [Benchmark]
    public object ToCandleResults() => h.ToCandleResults();
}
