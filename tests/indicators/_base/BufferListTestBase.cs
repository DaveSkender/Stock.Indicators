namespace Tests.Data;

/// <summary>
/// Base tests that all buffered list indicators should have for essential interfaces.
/// </summary>
/// <remarks>
/// Tests should additionally include AutoListPruning() or AutoCachePruning() test cases
/// when the buffer list includes non-standard Queue-based history caching.
/// </remarks>
public abstract class BufferListTestBase : TestBase
{
    /// <summary>
    /// Tests list auto-pruning behaviors in accordance with <see cref="BufferList{T}.MaxListSize"/>.
    /// This should include the proper pruning of internal buffers and caches, when applicable.
    /// </summary>
    public abstract void AutoListPruning();

    /// <summary>
    /// Tests clearing the list, queues/caches, and internals
    /// </summary>
    public abstract void ClearResetsState();
}

/// <summary>
/// Add this to buffer list tests for <see cref="IIncrementFromQuote" /> types.
/// </summary>
public interface ITestQuoteBufferList
{
    /// <summary>
    /// Tests adding individual quotes one-at-a-time
    /// </summary>
    abstract void AddQuotes();

    /// <summary>
    /// Tests adding a batch of quotes
    /// </summary>
    abstract void AddQuotesBatch();

    /// <summary>
    /// Tests if buffer list can be instantiated with initial quotes
    /// </summary>
    abstract void WithQuotesCtor();
}

/// <summary>
/// Add this to buffer list tests for <see cref="IIncrementFromChain" /> types.
/// </summary>
public interface ITestChainBufferList : ITestQuoteBufferList
{
    /// <summary>
    /// Tests adding IReusable type values one-at-a-time
    /// </summary>
    void AddReusableItems();

    /// <summary>
    /// Tests adding a batch of IReusable type values
    /// </summary>
    void AddReusableItemsBatch();

    /// <summary>
    /// Tests adding raw date/value pairs
    /// </summary>
    void AddDiscreteValues();
}

/// <summary>
/// Add this to buffer list tests for <see cref="IIncrementFromPairs" /> types that require paired inputs.
/// </summary>
public interface ITestPairsBufferList
{
    /// <summary>
    /// Tests adding paired IReusable type values one-at-a-time
    /// </summary>
    void AddReusablePairs();

    /// <summary>
    /// Tests adding a batch of paired IReusable type values
    /// </summary>
    void AddReusablePairsBatch();

    /// <summary>
    /// Tests adding raw date/value pairs
    /// </summary>
    void AddDiscretePairs();
}

/// <summary>
/// Defines a contract for a custom buffer list cache that supports pruning operations.
/// </summary>
public interface ITestCustomBufferListCache
{
    /// <summary>
    /// Tests custom buffer pruning behavior while list-level auto-pruning occurs simultaneously.
    /// </summary>
    void CustomBufferPruning();
}
