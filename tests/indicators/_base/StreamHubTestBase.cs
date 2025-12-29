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

    /// <summary>
    /// Optional method for indicator-specific provider history testing.
    /// Override in test classes to add custom validation for Insert/Remove scenarios.
    /// </summary>
    /// <remarks>
    /// This method can be called from QuoteObserver, ChainObserver, or ChainProvider tests
    /// to validate indicator-specific behavior during provider history mutations.
    /// Base implementation does nothing; subclasses override as needed.
    /// </remarks>
    protected virtual void AssertProviderHistoryIntegrity()
    {
        // Base implementation: no-op
        // Subclasses can override to add indicator-specific provider history validation

        // TODO: Is this used or needed?
    }
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

/// <summary>
/// Add this to dual-stream/pairs indicator tests.
/// </summary>
/// <remarks>
/// This interface is for testing indicators that require synchronized pairs of inputs,
/// such as Correlation, Beta, etc. Tests should validate timestamp synchronization,
/// dual-cache management, and proper error handling for mismatched inputs.
/// </remarks>
public interface ITestPairsObserver
{
    /// <summary>
    /// Tests hub capability with dual synchronized providers
    /// </summary>
    void PairsObserver_SynchronizedProviders_MatchesSeriesExactly();

    /// <summary>
    /// Tests hub behavior when timestamps don't match between providers
    /// </summary>
    void PairsObserver_TimestampMismatch_ThrowsInvalidQuotesException();
}
