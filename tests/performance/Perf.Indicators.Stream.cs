using BenchmarkDotNet.Attributes;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Performance;

public class IndicatorStreamFull
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
    public object GetEma()
    {
        QuoteProvider<Quote> provider = new();
        Ema<BasicData> ema = provider.GetEma(14);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();
        return ema.Results;
    }

    [Benchmark]
    public object GetSma()
    {
        QuoteProvider<Quote> provider = new();
        Sma<BasicData> sma = provider.GetSma(10);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();

        return sma.Results;
    }
}
