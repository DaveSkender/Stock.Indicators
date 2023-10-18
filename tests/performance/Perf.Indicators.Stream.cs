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
    public object GetAdlSeries() => q.GetAdl();

    [Benchmark]
    public object GetAdlManual()
    {
        List<Quote> quoteList = q.ToSortedList();
        Adl adl = new();

        for (int i = 0; i < quoteList.Count; i++)
        {
            adl.Increment(quoteList[i]);
        }

        return adl.Results;
    }

    [Benchmark]
    public object GetAdxSeries() => q.GetAdx(14);

    [Benchmark]
    public object GetAdxManual()
    {
        List<Quote> quoteList = q.ToSortedList();
        Adx adx = new(14);

        for (int i = 0; i < quoteList.Count; i++)
        {
            adx.Increment(quoteList[i]);
        }

        return adx.Results;
    }

    [Benchmark]
    public object GetEmaSeries() => q.GetEma(14);

    [Benchmark]
    public object GetEmaStream()
    {
        // TODO: refactor to exclude provider
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
        // TODO: refactor to exclude provider
        QuoteProvider provider = new();
        Sma observer = provider.GetSma(10);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();

        return observer.Results;
    }
}
