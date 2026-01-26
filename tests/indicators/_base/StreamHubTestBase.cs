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
    /// Verifies that the quote observer produces a series that matches the expected values
    /// exactly when handling warmup, late arrivals (rebuild), and removals.
    /// </summary>
    /// <remarks>Output series should identical to the reference series.</remarks>
    public abstract void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly();

    /// <summary>
    /// Tests cache pruning functionality
    /// </summary>
    public abstract void WithCachePruning_MatchesSeriesExactly();

    /// <summary>
    /// Tests hub-unique name string
    /// </summary>
    public abstract void ToStringOverride_ReturnsExpectedName();
}

public interface ITestQuoteObserver
{
    /// <summary>
    /// Tests hub compatibility with quote provider
    /// </summary>
    void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly();
}

/// <summary>
/// Add this to stream chainee indicator tests.
/// </summary>
public interface ITestChainObserver : ITestQuoteObserver
{
    /// <summary>
    /// Tests hub compatibility with chain providers
    /// </summary>
    void ChainObserver_ChainedProvider_MatchesSeriesExactly();
}

/// <summary>
/// Add this to all stream chainor indicator tests.
/// </summary>
public interface ITestChainProvider
{
    /// <summary>
    /// Tests hub capability as a chain provider
    /// </summary>
    void ChainProvider_MatchesSeriesExactly();
}
