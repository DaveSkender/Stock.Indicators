using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance;
#pragma warning disable SA1005 // Single line comments should begin with single space

// [MemoryDiagnoser]
public class IndicatorStreamPerformance
{
    private static IEnumerable<Quote> quotes;
    private static List<Quote> quotesList;
    private static List<(DateTime, double)> tpList;
    private static List<Quote> longish;
    private static List<Quote> onemill;

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        quotes = TestData.GetDefault();
        quotesList = quotes.ToSortedList();
        tpList = quotes.ToTuple(CandlePart.Close);
        longish = TestData.GetLongest().ToSortedList();
        onemill = TestData.GetRandom(1000000).ToSortedList();
    }

    // BENCHMARKS

    [Benchmark]
    public object GetEmaStdPreview() => quotes.GetEmaPreview(14);

    [Benchmark]
    public object GetEmaStdPreviewRawQuote()
    {
        EmaObs obsEma = new(null, 14);

        for (int i = 0; i < quotesList.Count; i++)
        {
            Quote q = quotesList[i];
            obsEma.Add(q);
        }

        return obsEma.Results;
    }

    [Benchmark]
    public object GetEmaStdPreviewRawTuple()
    {
        EmaObs obsEma = new(null, 14);

        for (int i = 0; i < tpList.Count; i++)
        {
            (DateTime, double) tp = tpList[i];
            obsEma.Add(tp);
        }

        return obsEma.Results;
    }

    [Benchmark]
    public object GetEma1M() => onemill.GetEma(14);

    //[Benchmark]
    //public object GetEma1MStream()
    //{
    //    EmaObs emaBase = new(14);

    //    for (int i = 0; i < onemill.Count; i++)
    //    {
    //        Quote q = onemill[i];
    //        emaBase.Add(q);
    //    }

    //    return emaBase.Results;
    //}

    [Benchmark]
    public object GetEmaStream11kQuote()
    {
        QuoteProvider provider = new();
        EmaObs obsEma = provider.ObsEma(14);

        for (int i = 0; i < 11000; i++)
        {
            provider.Add(longish[i]);
        }

        provider.EndTransmission();
        return obsEma.Results;
    }

    //[Benchmark]
    //public object GetEmaStream11kOHLC4base10k()
    //{
    //    EmaObs emaBase = lList
    //        .Take(10000)
    //        .Use(CandlePart.OHLC4)
    //        .InitEma(200);

    //    for (int i = 10000; i < 11000; i++)
    //    {
    //        (DateTime date, double value) q = lList[i]
    //            .ToTuple(CandlePart.OHLC4);

    //        emaBase.Add(q);
    //    }

    //    return emaBase.Results;
    //}

    //[Benchmark]
    //public object GetEmaStream11kOHLC4baseEmpty()
    //{
    //    EmaObs emaBase = new(200);

    //    for (int i = 0; i < lList.Count; i++)
    //    {
    //        (DateTime date, double value) q = lList[i]
    //            .ToTuple(CandlePart.OHLC4);

    //        emaBase.Add(q);
    //    }

    //    return emaBase.Results;
    //}
}
