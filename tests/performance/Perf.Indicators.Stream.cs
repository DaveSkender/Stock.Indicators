using BenchmarkDotNet.Attributes;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Performance;

public class IndicatorsStreaming
{
    private static IEnumerable<Quote> q;
    private static List<Quote> ql;

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        q = TestData.GetDefault();
        ql = q.ToSortedList();
    }

    // BENCHMARKS

    [Benchmark]
    public object GetEma() => q.GetEma(14);

    [Benchmark]
    public object GetEmaStream()
    {
        // todo: refactor to exclude provider
        QuoteProvider provider = new();
        Ema observer = provider.GetEma(14);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();

        return observer.Results;
    }

    [Benchmark]
    public object GetSma() => q.GetSma(10);

    [Benchmark]
    public object GetSmaStream()
    {
        // todo: refactor to exclude provider
        QuoteProvider provider = new();
        SmaObserver observer = provider.GetSma(10);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();

        return observer.Results;
    }
}
