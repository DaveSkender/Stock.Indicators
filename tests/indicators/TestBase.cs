using System.Globalization;
using System.Runtime.CompilerServices;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Other")]        // these use test data
[assembly: InternalsVisibleTo("Tests.Performance")]
namespace Test.Data;

[TestClass]
public abstract class TestBase  // base for all tests
{
    internal static readonly CultureInfo englishCulture = new("en-US", false);

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
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
}

/// <summary>
/// Base tests that all series indicators should have.
/// </summary>
[TestClass]
public abstract class StaticSeriesTestBase : TestBase
{
    [TestMethod]
    public abstract void Standard();

    [TestMethod]
    public abstract void BadData();

    [TestMethod]
    public abstract void NoQuotes();
}

/// <summary>
/// Base tests that all static indicators (series) should have.
/// </summary>
[TestClass]
public abstract class IncrementsTestBase : TestBase
{
    [TestMethod]
    public abstract void Standard();

    [TestMethod]
    public abstract void ValueBased();
}

/// <summary>
/// Base tests that all streamed indicators should have.
/// </summary>
[TestClass]
public abstract class StreamHubTestBase : TestBase  // default: quote observer
{
    [TestMethod]
    public abstract void QuoteObserver();

    [TestMethod]
    public abstract void CustomToString();
}

/// <summary>
/// Add this to stream chainee indicator tests.
/// </summary>
public interface ITestChainObserver
{
    [TestMethod]
    void ChainObserver();
}

/// <summary>
/// Add this to all stream chainor indicator tests.
/// </summary>
public interface ITestChainProvider
{
    [TestMethod]
    void ChainProvider();
}
