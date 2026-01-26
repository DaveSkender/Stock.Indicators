namespace Test.Base;

/// <summary>
/// Base tests that all streamed indicators should have.
/// </summary>
public abstract class StreamHubTestBase : TestBase  // default: quote observer
{
    internal const int removeAtIndex = 495;

    internal static readonly IReadOnlyList<Quote> RevisedQuotes
        = Quotes.Where(static (_, idx) => idx != removeAtIndex).ToList();
}

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
    /// <para>Tests cache pruning functionality.</para>
    /// <para>
    /// Expectations to validate here include:
    /// - Provider and observer caches respect configured `MaxCacheSize` limits.
    /// - The observable results reflect the final cached window (i.e. are
    ///   identical to a time-series computed from the provider's current
    ///   cached quotes).
    /// - Pruning does not corrupt incremental state (duplicates, removals,
    ///   and late-arrivals that fall inside the cached window remain
    ///   consistent).
    /// </para>
    /// <para>
    /// Implementations may additionally assert tolerable behaviour for
    /// late-arrivals that are older than the cached window (either ignored
    /// or cause a controlled rebuild). Tests should avoid assuming one
    /// rebuild policy universally — instead they must assert correctness
    /// for observable, well-defined invariants (cache sizing and final
    /// observable series equality against the cached-window recomputation).
    /// </para>
    /// </summary>
    void WithCachePruning_MatchesSeriesExactly();

    /// <summary>
    /// Tests hub-unique name string
    /// </summary>
    void ToStringOverride_ReturnsExpectedName();
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
