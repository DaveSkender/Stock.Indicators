using System.Runtime.InteropServices;

namespace Tests.Performance;
// ReSharper disable All

public class IndicatorStreamTests
{
    /*
     dotnet build -c Release

     Examples, to run cohorts:
     dotnet run -c Release -filter *IndicatorStreaming*
     dotnet run -c Release -filter *IndicatorStreaming.GetSma*
     */

    private static IEnumerable<Quote> q;
    private static List<Quote> ql;
    private readonly QuoteHub<Quote> provider = new();

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        q = TestData.GetDefault();
        ql = q.ToSortedList();

        ReadOnlySpan<Quote> spanQuotes
            = CollectionsMarshal.AsSpan(ql);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(spanQuotes[i]);
        }
    }

    // BENCHMARKS

    [Benchmark]
    public object AdlHub()
    {
        var hub = provider.ToAdl();
        return hub.Results;
    }

    [Benchmark]
    public object AlligatorHub()
    {
        var hub = provider.ToAlligator();
        return hub.Results;
    }
    
    [Benchmark]
    public object EmaHub()
    {
        var hub = provider.ToEma(14);
        return hub.Results;
    }

    [Benchmark]
    public object RenkoHub()
    {
        var hub = provider.ToRenko(2.5m);
        return hub.Results;
    }
    
    [Benchmark]
    public object SmaHub()
    {
        var hub = provider.ToSma(10);
        return hub.Results;
    }
}
