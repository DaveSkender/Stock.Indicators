using System.Globalization;
using System.Runtime.CompilerServices;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.PublicApi")]    // these use test data
[assembly: InternalsVisibleTo("Tests.Performance")]
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Test.Data;

public abstract class TestBase  // base for all tests
{
    internal static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    internal static readonly IReadOnlyList<Quote> Quotes = Data.GetDefault();
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
    public abstract void Standard();

    public abstract void BadData();

    public abstract void NoQuotes();
}

/// <summary>
/// Base tests that all static indicators (series) should have.
/// </summary>
public abstract class IncrementsTestBase : TestBase
{
    public abstract void FromQuote();

    public abstract void FromQuoteBatch();
}

/// <summary>
/// Base tests that all streamed indicators should have.
/// </summary>
public abstract class StreamHubTestBase : TestBase  // default: quote observer
{
    public abstract void QuoteObserver();

    public abstract void CustomToString();
}

/// <summary>
/// Add this to stream chainee indicator tests.
/// </summary>
public interface ITestChainObserver
{
    void ChainObserver();
}

/// <summary>
/// Add this to all stream chainor indicator tests.
/// </summary>
public interface ITestChainProvider
{
    void ChainProvider();
}
