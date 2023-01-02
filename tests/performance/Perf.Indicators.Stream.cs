using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance;

public class IndicatorStreamPerformance
{
    private static IEnumerable<Quote> quotes;
    private static List<Quote> quotesList;

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        quotes = TestData.GetDefault();
        quotesList = quotes.ToSortedList();
    }

    // BENCHMARKS

    [Benchmark]
    public object GetObsEma() => quotes.GetEma(14);

    [Benchmark]
    public object GetObsEmaStream()
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
    public object GetObsSma() => quotes.GetSma(10);

    [Benchmark]
    public object GetObsSmaStream()
    {
        // todo: refactor to exclude provider
        QuoteProvider provider = new();
        SmaObserver observer = provider.GetSma(10);

        for (int i = 0; i < quotesList.Count; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        return observer.Results;
    }
}
