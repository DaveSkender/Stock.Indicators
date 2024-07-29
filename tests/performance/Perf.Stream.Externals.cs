namespace Performance;

// STREAMING INDICATOR HUBS (EXTERNAL CACHE)

[ShortRunJob]
public class StreamExternal
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private readonly QuoteHub<Quote> provider = new();

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
        provider.ClearCache();
    }

    // BENCHMARKS

    // TODO: replace with external data cache model, when available

    [Benchmark(Baseline = true)]
    public object GetEma() => quotes.ToEma(14);

    [Benchmark]
    public object EmaHub() => provider.ToEma(14).Results;
}
