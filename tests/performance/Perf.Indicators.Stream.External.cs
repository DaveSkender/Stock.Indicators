namespace Tests.Performance;
// ReSharper disable All

public class IndicatorStreamExternal
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
    // TODO: replace with external data cache model, when available
    public object GetFoo()
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
}
