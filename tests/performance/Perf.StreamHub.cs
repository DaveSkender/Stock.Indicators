namespace Performance;

// STREAMING INDICATOR HUBS

[ShortRunJob]
public class StreamIndicators
{
    private static readonly IReadOnlyList<Quote> quotes
        = Data.GetDefault();

    private readonly QuoteHub<Quote> provider = new();  // prepopulated
    private readonly QuoteHub<Quote> supplier = new();  // empty

    /* SETUP/CLEANUP - runs before and after each.
     *
     * This Setup implies that each benchmark
     * will start with a prepopulated observable
     * QuoteHub provider.
     *
     * We do this because we want to measure
     * the performance of observer methods
     * without the overhead of the provider. */

    [GlobalSetup]
    public void Setup() => provider.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        provider.EndTransmission();
        provider.ClearCache(0);
    }

    // BENCHMARKS

    [Benchmark]
    public object AdlHub() => provider.ToAdl().Results;

    [Benchmark]
    public object AtrHub() => provider.ToAtr(14).Results;

    [Benchmark]
    public object AtrStopHub() => provider.ToAtrStop().Results;

    [Benchmark]
    public object AlligatorHub() => provider.ToAlligator().Results;

    [Benchmark]
    public object EmaHub() => provider.ToEma(14).Results;

    [Benchmark]
    public object EmaHub2()
    {
        EmaHub<Quote> observer = supplier.ToEma(14);

        for (int i = 0; i < quotes.Count; i++)
        {
            observer.OnNextArrival(quotes[i], i);
        }

        return observer.Results;
    }

    [Benchmark]
    public object QuoteHub() => provider.ToQuote().Results;

    [Benchmark]
    public object QuotePartHub() => provider.ToQuotePart(CandlePart.OHL3).Results;

    [Benchmark]
    public object RenkoHub() => provider.ToRenko(2.5m).Results;

    [Benchmark]
    public object SmaHub() => provider.ToSma(10).Results;

    [Benchmark]
    public object TrHub() => provider.ToTr().Results;
}
