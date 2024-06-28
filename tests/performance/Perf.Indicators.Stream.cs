using Tests.Performance.Config;

namespace Tests.Performance;

[Config(typeof(AntiVirusFriendlyConfig))]
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

    // SETUP

    [GlobalSetup]
    public void Setup()
    {
        q = TestData.GetDefault();
        ql = q.ToSortedList();
    }

    // BENCHMARKS

    [Benchmark]
    public void GetEma()
    {
        QuoteProvider<Quote> provider = new();
        Ema<Quote> ema = provider.ToEma(14);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();
    }

    [Benchmark]
    public void GetSma()
    {
        QuoteProvider<Quote> provider = new();
        Sma<Quote> sma = provider.ToSma(10);

        for (int i = 0; i < ql.Count; i++)
        {
            provider.Add(ql[i]);
        }

        provider.EndTransmission();
    }
}
