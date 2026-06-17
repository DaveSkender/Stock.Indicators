namespace Performance;

// STREAMING INDICATOR HUBS (EXTERNAL CACHE)

[ShortRunJob]
public class StreamExternal
{
    private static readonly IReadOnlyList<Bar> bars = Data.GetDefault();
    private const int n = 14;

    private readonly BarHub barHub = new();

    /* SETUP/CLEANUP - runs before and after each.
     *
     * This Setup implies that each benchmark
     * will start with a prepopulated observable
     * BarHub provider.
     *
     * We do this because we want to measure
     * the performance of observer methods
     * without the overhead of the provider. */

    [GlobalSetup]
    public void Setup() => barHub.Add(bars);

    [GlobalCleanup]
    public void Cleanup()
    {
        barHub.EndTransmission();
        barHub.Cache.Clear();
    }

    // BENCHMARKS

    // TODO: replace with external data cache model, when available

    [Benchmark(Baseline = true)]
    public object EmaSeries() => bars.ToEma(n);

    [Benchmark]
    public object EmaStream() => barHub.ToEmaHub(n).Results;
}
