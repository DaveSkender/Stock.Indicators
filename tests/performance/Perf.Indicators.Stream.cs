using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance;

[MemoryDiagnoser]
public class IndicatorStreamPerformance
{
    private static IEnumerable<Quote> h;
    private static List<Quote> onemill;
    private static List<Quote> lList;

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        h = TestData.GetDefault();
        lList = TestData.GetLongest().ToList();
        onemill = TestData.GetRandom(1000000).ToList();
    }

    // BENCHMARKS

    [Benchmark]
    public object GetEma1M() => onemill.GetEma(14);

    [Benchmark]
    public object GetEma1MStream()
    {
        EmaBase emaBase = new(14);

        for (int i = 0; i < onemill.Count; i++)
        {
            Quote q = onemill[i];
            emaBase.Add(q);
        }

        return emaBase.Results;
    }

    [Benchmark]
    public object GetEmaStream11kQuote()
    {
        EmaBase emaBase = lList
            .Take(10000)
            .InitEma(200);

        for (int i = 10000; i < 11000; i++)
        {
            Quote q = lList[i];
            emaBase.Add(q);
        }

        return emaBase.Results;
    }

    [Benchmark]
    public object GetEmaStreamOHLC4base10k()
    {
        EmaBase emaBase = lList
            .Take(10000)
            .Use(CandlePart.OHLC4)
            .InitEma(200);

        for (int i = 10000; i < 11000; i++)
        {
            (DateTime date, double value) q = lList[i]
                .ToTuple(CandlePart.OHLC4);

            emaBase.Add(q);
        }

        return emaBase.Results;
    }

    [Benchmark]
    public object GetEmaStreamOHLC4baseEmpty()
    {
        EmaBase emaBase = new(200);

        for (int i = 0; i < lList.Count; i++)
        {
            (DateTime date, double value) q = lList[i]
                .ToTuple(CandlePart.OHLC4);

            emaBase.Add(q);
        }

        return emaBase.Results;
    }
}
