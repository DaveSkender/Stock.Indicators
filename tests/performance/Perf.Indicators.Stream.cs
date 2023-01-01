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

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        quotes = TestData.GetDefault();
        quotesList = quotes.ToSortedList();
        tpList = quotes.ToTuple(CandlePart.Close);
        onemill = TestData.GetRandom(1000000).ToSortedList();
    }

    // BENCHMARKS

    [Benchmark]
    public object GetEmaStd() => quotes.GetEma(14);

    public object GetEmaStdStream()
    {
        // todo: refactor to exclude provider
        QuoteProvider provider = new();

        EmaObserver observer
            = provider.GetEma(14);

        IEnumerable<EmaResult> results
            = observer.Results;

        for (int i = 0; i < quotesList.Count; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        return results;
    }

    [Benchmark]
    [Obsolete("This is only for testing.", false)]
    public object GetEmaStdStreamRawQuote()
    {
        EmaObserver obsEma = new(null, 14);

        for (int i = 0; i < quotesList.Count; i++)
        {
            Quote q = quotesList[i];
            obsEma.Add(q);
        }

        // obsEma.Unsubscribe();
        return obsEma.Results;
    }

    [Benchmark]
    [Obsolete("This is only for testing.", false)]
    public object GetEmaStdStreamRawTuple()
    {
        EmaObserver obsEma = new(null, 14);

        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime, double) tp = tpList[i];
            obsEma.Add(tp);
        }

        // obsEma.Unsubscribe();
        return obsEma.Results;
    }

    [Benchmark]
    public object GetEma1M() => onemill.GetEma(14);

    [Benchmark]
    public object GetEma1MStream()
    {
        QuoteProvider provider = new();

        EmaObserver observer
            = provider.GetEma(14);

        IEnumerable<EmaResult> results
            = observer.Results;

        for (int i = 0; i < onemill.Count; i++)
        {
            provider.Add(onemill[i]);
        }

        provider.EndTransmission();

        return results;
    }
}
