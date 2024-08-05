namespace Performance;

// TIME-SERIES INDICATORS

[ShortRunJob]
public class Incrementals
{
    private static readonly IReadOnlyList<Quote> quotes
       = Data.GetDefault();

    private static readonly List<Quote> quotesList
       = quotes
        .ToSortedList();

    private static readonly double[] primatives
       = quotes
        .Select(x => x.Value)
        .ToArray();

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
        provider.ClearCache();
    }

    [Benchmark]
    public object EmaIncRusBatch()
    {
        EmaInc sut = new(14) { reusables };
        return sut;
    }

    [Benchmark]
    public object EmaIncRusItem()
    {
        EmaInc sut = new(14);

        for (int i = 0; i < reusables.Count; i++)
        {
            sut.Add(reusables[i]);
        }

        return sut;
    }

    [Benchmark]
    public object EmaIncRusSplit()
    {
        EmaInc sut = new(14);

        for (int i = 0; i < reusables.Count; i++)
        {
            sut.Add(reusables[i].Timestamp, reusables[i].Value);
        }

        return sut;
    }

    [Benchmark]
    public object EmaIncQotBatch()
    {
        EmaInc sut = new(14) { quotes };
        return sut;
    }

    [Benchmark]
    public object EmaIncQot()
    {
        EmaInc sut = new(14);

        for (int i = 0; i < quotes.Count; i++)
        {
            sut.Add(quotes[i]);
        }

        return sut;
    }

    [Benchmark]
    public object EmaIncPrmBatch()
    {
        EmaIncPrimitive sut = new(14) { primatives };
        return sut;
    }

    [Benchmark]
    public object EmaIncPrm()
    {
        EmaIncPrimitive sut = new(14);

        for (int i = 0; i < primatives.Length; i++)
        {
            sut.Add(primatives[i]);
        }

        return sut;
    }

    // TIME-SERIES EQUIVALENTS

    [Benchmark(Baseline = true)]
    public object EmaSeriesEqiv() => quotesList.CalcEma(14);

    [Benchmark]
    public object EmaSeriesOrig() => quotes.ToEma(14);

    [Benchmark]
    public object EmaIncremEqiv()
    {
        EmaInc ema = new(14) { quotes.ToSortedList() };
        return ema;
    }

    [Benchmark]
    public object EmaStreamEqiv() => provider.ToEma(14).Results;
}
