namespace Test.Base;

/// <summary>
/// Base tests that all streamed indicators should have.
/// </summary>
public abstract class StreamHubTestBase : TestBase  // default: quote observer
{
    internal const int removeAtIndex = 495;

    internal static readonly IReadOnlyList<Quote> RevisedQuotes
        = Quotes.Where(static (_, idx) => idx != removeAtIndex).ToList();

    /// <summary>
    /// Tests hub-unique name string
    /// </summary>
    public abstract void ToStringOverride_ReturnsExpectedName();
}

/// <summary>
/// Add this to stream indicator tests
/// (all get this directly or inherited from IChainObserver).
/// </summary>
public interface ITestQuoteObserver
{
    /// <summary>
    /// <para>Tests hub compatibility with a quote provider.</para>
    /// <para>
    /// Implementations should verify the observer reproduces the canonical
    /// time-series given a realistic quote stream, including warmup
    /// behaviour, late arrivals, duplicate re-sends and removals. When
    /// applicable, include checks that the final observed series is
    /// identical to the equivalent static-series computation.
    /// </para>
    /// </summary>
    void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly();

    /// <summary>
    /// <para>Tests hub cache pruning behavior.</para>
    /// <para>
    /// Implementations should verify that when a QuoteHub has MaxCacheSize set,
    /// the observer produces results matching the last N results from the full
    /// series computation (where N = MaxCacheSize), not a recomputation on just
    /// the cached quotes which would have different warmup.
    /// </para>
    /// </summary>
    void WithCachePruning_MatchesSeriesExactly();
}

/// <summary>
/// Add this to stream chainee indicator tests.
/// </summary>
public interface ITestChainObserver : ITestQuoteObserver
{
    /// <summary>
    /// <para>
    /// Tests hub compatibility when the hub is chained to another provider
    /// (for example: QuoteHub -> SmaHub -> SubjectHub).
    /// </para>
    /// <para>
    /// Implementations should verify that chained providers produce the
    /// same final series as the equivalent time-series pipeline and that
    /// cache/pruning behaviour of upstream providers does not corrupt
    /// downstream results.
    /// </para>
    /// </summary>
    void ChainObserver_ChainedProvider_MatchesSeriesExactly();
}

/// <summary>
/// Add this to all stream chainor indicator tests.
/// </summary>
public interface ITestChainProvider
{
    /// <summary>
    /// <para>Tests hub capability as a chain provider.</para>
    /// <para>
    /// Implementations should confirm that the hub can act as a provider
    /// for downstream hubs and that chaining preserves series equality,
    /// correct warmup handling, and predictable cache-pruning behaviour
    /// when upstream caches are constrained.
    /// </para>
    /// </summary>
    void ChainProvider_MatchesSeriesExactly();
}
