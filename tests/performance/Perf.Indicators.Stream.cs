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
    private static ReadOnlySpan<Quote> ql
        = CollectionsMarshal.AsSpan(q.ToSortedList());

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        q = TestData.GetDefault();
    }

    // BENCHMARKS

    [Benchmark]
    public object GetAdl()
    {
        QuoteHub<Quote> provider = new();
        AdlHub<Quote> adl = provider.ToAdl();

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();
        return adl.Results;
    }

    [Benchmark]
    public object GetAlligator()
    {
        QuoteHub<Quote> provider = new();
        AlligatorHub<Quote> alligator = provider.ToAlligator();

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();
        return alligator.Results;
    }
    
    [Benchmark]
    public object GetEma()
    {
        QuoteHub<Quote> provider = new();
        EmaHub<Quote> ema = provider.ToEma(14);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();
        return ema.Results;
    }

    [Benchmark]
    public object GetRenko()
    {
        QuoteHub<Quote> provider = new();
        RenkoHub<Quote> renko = provider.ToRenko(2.5);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();
        return renko.Results;
    }
    
    [Benchmark]
    public object GetSma()
    {
        QuoteHub<Quote> provider = new();
        SmaHub<Quote> sma = provider.ToSma(10);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();
        return sma.Results;
    }
}
