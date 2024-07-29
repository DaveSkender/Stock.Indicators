namespace Performance;

// TIME-SERIES INDICATORS

[ShortRunJob]
public class Incrementals
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly double[] values = Data.GetDefault().Select(x => x.Value).ToArray();
    private readonly QuoteHub<Quote> provider = new();

    [GlobalSetup]
    public void Setup() => provider.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        provider.EndTransmission();
        provider.ClearCache();
    }

    [Benchmark]
    public object EmaListQ()
    {
        EmaList<Quote> sut = new(14);

        for (int i = 0; i < quotes.Count; i++)
        {
            sut.Add(quotes[i]);
        }

        return sut;
    }

    [Benchmark]
    public object EmaListP()
    {
        EmaList<Quote> sut = new(14);

        for (int i = 0; i < quotes.Count; i++)
        {
            sut.Add(quotes[i].Timestamp, quotes[i].Value);
        }

        return sut;
    }

    [Benchmark]
    public object EmaArray()
    {
        EmaArray sut = new(14);

        for (int i = 0; i < values.Length; i++)
        {
            sut.Add(values[i]);
        }

        return sut;
    }

    [Benchmark]
    public object EmaSeries() => quotes.ToEma(14);

    // [Benchmark]
    // public object EmaStream() => provider.ToEma(14).Results;
}
