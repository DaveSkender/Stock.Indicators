namespace Performance;

// TIME-SERIES INDICATORS

[ShortRunJob]
public class Incrementals
{
    private static readonly IReadOnlyList<Quote> quotes
       = Data.GetDefault();

    private static readonly IReadOnlyList<IReusable> reusables
       = quotes
        .Cast<IReusable>()
        .ToList();

    private readonly QuoteHub<Quote> provider = new();

    [GlobalSetup]
    public void Setup() => provider.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        provider.EndTransmission();
        provider.Cache.Clear();
    }

    [Benchmark]
    public object EmaIncRusBatch()
    {
        EmaList sut = new(14) { reusables };
        return sut;
    }

    [Benchmark]
    public object EmaIncRusItem()
    {
        EmaList sut = new(14);

        for (int i = 0; i < reusables.Count; i++)
        {
            sut.Add(reusables[i]);
        }

        return sut;
    }

    [Benchmark]
    public object EmaIncRusSplit()
    {
        EmaList sut = new(14);

        for (int i = 0; i < reusables.Count; i++)
        {
            sut.Add(reusables[i].Timestamp, reusables[i].Value);
        }

        return sut;
    }

    [Benchmark]
    public object EmaIncQotBatch()
    {
        EmaList sut = new(14) { quotes };
        return sut;
    }

    [Benchmark]
    public object EmaIncQot()
    {
        EmaList sut = new(14);

        for (int i = 0; i < quotes.Count; i++)
        {
            sut.Add(quotes[i]);
        }

        return sut;
    }

    // TIME-SERIES EQUIVALENTS

    [Benchmark]
    public object EmaSeries() => quotes.ToEma(14);

    [Benchmark]
    public object EmaIncrem()
    {
        EmaList ema = new(14) { quotes };
        return ema;
    }

    [Benchmark]
    public object EmaStream() => provider.ToEma(14).Results;
}
