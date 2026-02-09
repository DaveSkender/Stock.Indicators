namespace Performance;

// STREAMING INDICATOR HUBS (EXTERNAL CACHE)

[ShortRunJob]
public class StreamExternal
{
    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private const int n = 14;

    private readonly QuoteHub quoteHub = new();

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
    public void Setup() => quoteHub.Add(quotes);

    [GlobalCleanup]
    public void Cleanup()
    {
        quoteHub.EndTransmission();
        quoteHub.Cache.Clear();
    }

    // BENCHMARKS

    // TODO: replace with external data cache model, when available

    [Benchmark(Baseline = true)]
    public object EmaSeries() => quotes.ToEma(n);

    [Benchmark]
    public object EmaStream() => quoteHub.ToEmaHub(n).Results;
}
