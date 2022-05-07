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
    private static IEnumerable<ObvResult> obv;

    [GlobalSetup]
    public void Setup()
    {
        h = TestData.GetDefault();
    }

    [Benchmark]
    public object SortToList()
    {
        return h.SortToList();
    }

    [Benchmark]
    public object ToListQuoteD()
    {
        return h.ToQuoteD();
    }

    [Benchmark]
    public object Validate()
    {
        return h.Validate();
    }

    [GlobalSetup(Targets = new[] { nameof(Aggregate) })]
    public void SetupIntraday()
    {
        i = TestData.GetIntraday();
    }

    [Benchmark]
    public object Aggregate()
    {
        return i.Aggregate(PeriodSize.FifteenMinutes);
    }

    [Benchmark]
    public object ToBasicData()
    {
        return h.ToBasicClass();
    }

    [Benchmark]
    public object ToBasicTuple()
    {
        return h.ToBasicTuple();
    }

    [Benchmark]
    public object ToCandleResults()
    {
        return h.ToCandleResults();
    }

    [GlobalSetup(Targets = new[] { nameof(ToQuotes) })]
    public void SetupQuotes()
    {
        h = TestData.GetDefault();
        obv = h.GetObv();
    }

    [Benchmark]
    public object ToQuotes()
    {
        return obv.ToQuotes();
    }
}
