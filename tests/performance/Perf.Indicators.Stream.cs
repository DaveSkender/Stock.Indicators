namespace Tests.Performance;
// ReSharper disable All

public class IndicatorStreamTests
{
    private static readonly List<Quote> ql
        = TestData.GetDefault().ToSortedList();

    private readonly QuoteHub<Quote> provider = new();

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }
    }

    // BENCHMARKS

    [Benchmark]
    public object AdlHub()
    {
        AdlHub<Quote> hub = provider.ToAdl();
        return hub.Results;
    }

    [Benchmark]
    public object AlligatorHub()
    {
        AlligatorHub<Quote> hub = provider.ToAlligator();
        return hub.Results;
    }

    [Benchmark]
    public object EmaHub()
    {
        EmaHub<Quote> hub = provider.ToEma(14);
        return hub.Results;
    }

    [Benchmark]
    public object QuoteHub()  // redistribution
    {
        QuoteHub<Quote> hub = provider.ToQuote();
        return hub.Results;
    }

    [Benchmark]
    public object QuotePartHub()
    {
        QuotePartHub<Quote> hub = provider.ToQuotePart(CandlePart.OHL3);
        return hub.Results;
    }

    [Benchmark]
    public object RenkoHub()
    {
        RenkoHub<Quote> hub = provider.ToRenko(2.5m);
        return hub.Results;
    }

    [Benchmark]
    public object SmaHub()
    {
        SmaHub<Quote> hub = provider.ToSma(10);
        return hub.Results;
    }
}
