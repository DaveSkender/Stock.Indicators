using BenchmarkDotNet.Attributes;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Performance;

public class IndicatorStreamDirect
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
        Ema ema = new(14);

        for (int i = 0; i < ql.Count; i++)
        {
            ema.Add(ql[i]);
        }

        return ema.Results;
    }

    [Benchmark]
    public object GetSma()
    {
        Sma sma = new(10);

        for (int i = 0; i < ql.Count; i++)
        {
            sma.Add(ql[i]);
        }

        return sma.Results;
    }
}
