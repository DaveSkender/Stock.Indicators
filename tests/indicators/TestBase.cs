using System.Globalization;
using System.Runtime.CompilerServices;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Other")]        // these use test data
[assembly: InternalsVisibleTo("Tests.Performance")]
namespace Test.Data;

[TestClass]
public abstract class TestBase
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
}

/// <summary>
/// Base tests that all static indicators (series) should have.
/// You'll get a placeholder result where not implemented.
/// </summary>
[TestClass]
public abstract class SeriesTestBase : TestBase
{
    internal readonly DateTime EvalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);

    [TestMethod]
    public abstract void Standard();

    [TestMethod]
    public abstract void BadData();

    [TestMethod]
    public abstract void NoQuotes();
}

/// <summary>
/// Base tests that all streamed indicators should have.
/// You'll get a placeholder result where not implemented.
/// </summary>
[TestClass]
public abstract class StreamTestBase : TestBase
{
    [TestMethod]
    public abstract void QuoteObserver();

    [TestMethod]
    public abstract void CustomToString();
}

/// <summary>
/// Add this to stream chainee indicator tests.
/// You'll get a placeholder result where not implemented.
/// </summary>
public interface ITestChainObserver
{
    [TestMethod]
    void ChainObserver();
}

/// <summary>
/// Add this to all stream chainor indicator tests.
/// You'll get a placeholder result where not implemented.
/// </summary>
public interface ITestChainProvider
{
    [TestMethod]
    void ChainProvider();
}
