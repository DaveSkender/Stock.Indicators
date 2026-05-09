namespace Test.Base;

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
    /// </summary>
    /// <remarks>
    /// Tests should include the proper pruning of internal buffers and caches, when applicable.
    /// </remarks>
    public abstract void PruneList_OverMaxListSize_AutoAdjustsListAndBuffers();

    /// <summary>
    /// Tests clearing the list, queues/caches, and internals
    /// </summary>
    public abstract void Clear_WithState_ResetsState();
}

/// <summary>
/// Add this to buffer list tests for <see cref="IIncrementFromQuote" /> types.
/// </summary>
public interface ITestQuoteBufferList
{
    /// <summary>
    /// Tests adding individual quotes one-at-a-time
    /// </summary>
    abstract void AddQuote_IncrementsResults();

    /// <summary>
    /// Tests adding a batch of quotes
    /// </summary>
    abstract void AddQuotesBatch_IncrementsResults();

    /// <summary>
    /// Tests if buffer list can be instantiated with initial quotes
    /// </summary>
    abstract void QuotesCtor_OnInstantiation_IncrementsResults();
}

/// <summary>
/// Add this to buffer list tests for <see cref="IIncrementFromChain" /> types.
/// </summary>
public interface ITestChainBufferList : ITestQuoteBufferList
{
    /// <summary>
    /// Tests adding IReusable type values one-at-a-time
    /// </summary>
    void AddReusableItem_IncrementsResults();

    /// <summary>
    /// Tests adding a batch of IReusable type values
    /// </summary>
    void AddReusableItemBatch_IncrementsResults();

    /// <summary>
    /// Tests adding raw date/value elements
    /// </summary>
    void AddDateAndValue_IncrementsResults();
}

/// <summary>
/// Defines a contract for a custom buffer list cache that supports pruning operations.
/// </summary>
public interface ITestCustomBufferListCache
{
    /// <summary>
    /// Tests custom buffer pruning behavior while list-level auto-pruning occurs simultaneously.
    /// </summary>
    void CustomBuffer_OverMaxListSize_AutoAdjustsListAndBuffers();
}
