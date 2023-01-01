using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance;

public class IndicatorStreamPerformance
{
    private static IEnumerable<Quote> quotes;
    private static List<Quote> quotesList;
    private static List<(DateTime, double)> tpList;
    private static List<Quote> onemill;
    private static List<(DateTime, double)> tpMill;

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        quotes = TestData.GetDefault();
        quotesList = quotes.ToSortedList();
        tpList = quotes.ToTuple(CandlePart.Close);
        onemill = TestData.GetRandom(1000000).ToSortedList();
        tpMill = onemill.ToTuple(CandlePart.Close);
    }

    // BENCHMARKS

    [Benchmark]
    public object GetEmaStd() => quotes.GetEma(14);

    [Benchmark]
    public object GetEmaStdNoProviderPresort()
    {
        System.Collections.ObjectModel.Collection<(DateTime, double)> quoteCollection = quotes
            .ToTupleCollection(CandlePart.Close);

        EmaObserver observer = new(null, 14);

        for (int i = 0; i < quoteCollection.Count; i++)
        {
            observer.Add(quoteCollection[i]);
        }

        return observer.Results;
    }

    [Benchmark]
    public object GetEmaStdStream()
    {
        // todo: refactor to exclude provider
        QuoteProvider provider = new();
        EmaObserver observer = provider.GetEma(14);

        for (int i = 0; i < quotesList.Count; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        return observer.Results;
    }

    [Benchmark]
    public object GetEmaStdNoProviderTuple()
    {
        EmaObserver observer = new(null, 14);

        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime, double) tp = tpList[i];
            observer.Add(tp);
        }

        return observer.Results;
    }

    [Benchmark]
    public object GetEmaStd1M() => onemill.GetEma(14);

    [Benchmark]
    public object GetEmaStd1MStream()
    {
        QuoteProvider provider = new();
        EmaObserver observer = provider.GetEma(14);

        for (int i = 0; i < onemill.Count; i++)
        {
            provider.Add(onemill[i]);
        }

        provider.EndTransmission();

        return observer.Results;
    }

    [Benchmark]
    public object GetEmaStd1MNoProviderTuple()
    {
        EmaObserver observer = new(null, 14);

        for (int i = 0; i < tpMill.Count; i++)
        {
            observer.Add(tpMill[i]);
        }

        return observer.Results;
    }
}
