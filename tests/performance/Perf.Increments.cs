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
        EmaInc sut = new(14);
        sut.AddValues(reusables);
        return sut;
    }

    [Benchmark]
    public object EmaIncRus()
    {
        EmaInc sut = new(14);

        for (int i = 0; i < reusables.Count; i++)
        {
            sut.AddValue(reusables[i].Timestamp, reusables[i].Value);
        }

        return sut;
    }

    [Benchmark]
    public object EmaIncQotBatch()
    {
        EmaInc sut = new(14);
        sut.AddValues(quotes);
        return sut;
    }

    [Benchmark]
    public object EmaIncQot()
    {
        EmaInc sut = new(14);

        for (int i = 0; i < quotes.Count; i++)
        {
            sut.AddValue(quotes[i]);
        }

        return sut;
    }

    [Benchmark]
    public object EmaIncPrmBatch()
    {
        EmaIncPrimitive sut = new(14);
        sut.AddValues(primatives);
        return sut;
    }

    [Benchmark]
    public object EmaIncPrm()
    {
        EmaIncPrimitive sut = new(14);

        for (int i = 0; i < primatives.Length; i++)
        {
            sut.AddValue(primatives[i]);
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
        EmaInc ema = new(14);
        ema.AddValues(quotes.ToSortedList());
        return ema;
    }

    [Benchmark]
    public object EmaStreamEqiv() => provider.ToEma(14).Results;
}
