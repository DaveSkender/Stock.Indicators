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

    private static readonly double[] primitives
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
        provider.ClearCache(0);
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

    [Benchmark]
    public object EmaIncPrmBatch()
    {
        EmaIncPrimitive sut = new(14) { primitives };
        return sut;
    }

    [Benchmark]
    public object EmaIncPrm()
    {
        EmaIncPrimitive sut = new(14);

        for (int i = 0; i < primitives.Length; i++)
        {
            sut.Add(primitives[i]);
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
        EmaList ema = new(14) { quotes.ToSortedList() };
        return ema;
    }

    [Benchmark]
    public object EmaStreamEqiv() => provider.ToEma(14).Results;

    [Benchmark(Baseline = true)]
    public object SmaArrOrig()
    {
        int periods = 20;
        double[] results = new double[primitives.Length];

        for (int i = 0; i < primitives.Length; i++)
        {
            if (i < periods - 1)
            {
                results[i] = double.NaN;
                continue;
            }

            double sum = 0;
            for (int w = i - periods + 1; w <= i; w++)
            {
                sum += primitives[i];
            }
            results[i] = sum / periods;
        }
        return results;
    }

    [Benchmark]
    public object SmaArrSimd() => primitives.CalcSma(20);
}
