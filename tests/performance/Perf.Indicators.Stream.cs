using BenchmarkDotNet.Attributes;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Performance;

public class IndicatorsStreaming
{
    /*
     dotnet build -c Release

     Examples, to run cohorts:
     dotnet run -c Release -filter *IndicatorStreaming*
     dotnet run -c Release -filter *IndicatorStreaming.GetSma*
     */

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
    public object GetEmaSeries() => q.GetEma(14);

    [Benchmark]
    public object GetEmaManual()
    {
        Ema ema = new(14);

        for (int i = 0; i < ql.Count; i++)
        {
            ema.Add(ql[i]);
        }

        return ema.Results;
    }

    [Benchmark]
    public object GetEmaStream()
    {
        QuoteProvider<Quote> provider = new();
        Ema ema = provider.GetEma(14);

        for (int i = 0; i < ql.Count; i++)
        {
            ema.Add(ql[i]);
        }

        provider.EndTransmission();
        return ema.Results;
    }

    [Benchmark]
    public object GetSmaSeries() => q.GetSma(10);

    [Benchmark]
    public object GetSmaManual()
    {
        Sma sma = new(10);

        for (int i = 0; i < ql.Count; i++)
        {
            sma.Add(ql[i]);
        }

        return sma.Results;
    }

    [Benchmark]
    public object GetSmaStream()
    {
        QuoteProvider<Quote> provider = new();
        Sma observer = provider.GetSma(10);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();

        return observer.Results;
    }
}
