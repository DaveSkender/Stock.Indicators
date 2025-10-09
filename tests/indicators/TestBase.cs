using System.Globalization;
using System.Runtime.CompilerServices;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.PublicApi")]    // these use test data
[assembly: InternalsVisibleTo("Tests.Performance")]
[assembly: InternalsVisibleTo("BaselineGenerator")]
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Test.Data;

public abstract class TestBase  // base for all tests
{
    internal static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    internal static readonly IReadOnlyList<Quote> Quotes = Data.GetDefault();
    internal static readonly IReadOnlyList<Quote> Intraday = Data.GetIntraday();
    internal static readonly IReadOnlyList<Quote> OtherQuotes = Data.GetCompare();
    internal static readonly IReadOnlyList<Quote> BadQuotes = Data.GetBad();
    internal static readonly IReadOnlyList<Quote> BigQuotes = Data.GetTooBig();
    internal static readonly IReadOnlyList<Quote> LongishQuotes = Data.GetLongish();
    internal static readonly IReadOnlyList<Quote> LongestQuotes = Data.GetLongest();
    internal static readonly IReadOnlyList<Quote> MismatchQuotes = Data.GetMismatch();
    internal static readonly IReadOnlyList<Quote> Noquotes = [];
    internal static readonly IReadOnlyList<Quote> Onequote = Data.GetDefault(1);
    internal static readonly IReadOnlyList<Quote> RandomQuotes = Data.GetRandom(1000);
    internal static readonly IReadOnlyList<Quote> ZeroesQuotes = Data.GetZeros();

    protected static readonly double DoublePrecision = 1E-13;

    protected static readonly DateTime EvalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture);
}

/// <summary>
/// Base tests that all series indicators should have.
/// </summary>
public abstract class StaticSeriesTestBase : TestBase
{
    /// <summary>
    /// Tests default use case and parameters arguments
    /// </summary>
    public abstract void Standard();

    /// <summary>
    /// Tests proper handling of incompatible quote data
    /// </summary>
    public abstract void BadData();

    /// <summary>
    /// Tests that empty quotes sets return empty results set
    /// </summary>
    public abstract void NoQuotes();
}

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
    /// Tests adding individual quotes one-at-a-time
    /// </summary>
    public abstract void AddQuotes();

    /// <summary>
    /// Tests adding a batch of quotes
    /// </summary>
    public abstract void AddQuotesBatch();

    /// <summary>
    /// Tests if buffer list can be instantiated with initial quotes
    /// </summary>
    public abstract void WithQuotesCtor();

    /// <summary>
    /// Tests normal list auto-pruning behaviors
    /// </summary>
    public abstract void AutoListPruning();

    /// <summary>
    /// Tests clearing the list, queues/caches, and internals
    /// </summary>
    public abstract void ClearResetsState();
}

/// <summary>
/// Base setup for regression tests
/// </summary>
public abstract class RegressionTestBase<TResult> : TestBase
    where TResult : ISeries
{
    protected IReadOnlyList<TResult> Expected { get; init; }

    private static QuoteHub<Quote>? _sharedQuoteHub;
    private static bool _quoteHubInitialized;

    protected static QuoteHub<Quote> quoteHub
    {
        get
        {
            if (!_quoteHubInitialized)
            {
                _sharedQuoteHub = new QuoteHub<Quote>();
                _sharedQuoteHub.Add(Quotes);
                _quoteHubInitialized = true;
            }
            return _sharedQuoteHub!;
        }
    }

    protected RegressionTestBase(string filename)
    {
        Expected = Data.Results<TResult>(filename);
    }

    public abstract void Series();
    public abstract void Buffer();
    public abstract void Stream();
}

/// <summary>
/// Add this to buffer list tests for <see cref="IBufferReusable" /> types.
/// </summary>
public interface ITestReusableBufferList
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
/// Add this to buffer list tests where a non-standard cache is used instead of the standard <see cref="Queue{T}"/> implementation.
/// </summary>
public interface ITestNonStandardBufferListCache
{
    /// <summary>
    /// Tests non-standard internal cache pruning behavior while list-level auto-pruning occurs simultaneously.
    /// </summary>
    void AutoBufferPruning();
}

/// <summary>
/// Base tests that all streamed indicators should have.
/// </summary>
public abstract class StreamHubTestBase : TestBase  // default: quote observer
{
    /// <summary>
    /// Tests hub compatibility with quote provider
    /// </summary>
    public abstract void QuoteObserver();

    /// <summary>
    /// Tests hub-unique name string
    /// </summary>
    public abstract void CustomToString();
}

/// <summary>
/// Add this to stream chainee indicator tests.
/// </summary>
public interface ITestChainObserver
{
    /// <summary>
    /// Tests hub compatibility with chain providers
    /// </summary>
    void ChainObserver();
}

/// <summary>
/// Add this to all stream chainor indicator tests.
/// </summary>
public interface ITestChainProvider
{
    /// <summary>
    /// Tests hub capability as a chain provider
    /// </summary>
    void ChainProvider();
}
