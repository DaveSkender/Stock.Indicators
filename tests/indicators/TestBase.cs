using System.Globalization;
using System.Runtime.CompilerServices;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Other")]
[assembly: InternalsVisibleTo("Tests.Performance")]
[assembly: InternalsVisibleTo("Observe.Streaming")]
namespace Tests.Common;

[TestClass]
public abstract class TestBase
{
    internal static readonly CultureInfo englishCulture = new("en-US", false);

    internal static IEnumerable<Quote> Quotes = [];
    internal static IEnumerable<Quote> OtherQuotes = [];
    internal static IEnumerable<Quote> BadQuotes = [];
    internal static IEnumerable<Quote> BigQuotes = [];
    internal static IEnumerable<Quote> LongishQuotes = [];
    internal static IEnumerable<Quote> LongestQuotes = [];
    internal static IEnumerable<Quote> MismatchQuotes = [];
    internal static IEnumerable<Quote> Noquotes = [];
    internal static IEnumerable<Quote> Onequote = [];
    internal static IEnumerable<Quote> RandomQuotes = [];
    internal static IEnumerable<Quote> ZeroesQuotes = [];

    internal TestBase()
    {
        try
        {
            Quotes = TestData.GetDefault();
            OtherQuotes = TestData.GetCompare();
            BadQuotes = TestData.GetBad();
            BigQuotes = TestData.GetTooBig();
            LongishQuotes = TestData.GetLongish();
            LongestQuotes = TestData.GetLongest();
            MismatchQuotes = TestData.GetMismatch();
            Noquotes = new List<Quote>();
            Onequote = TestData.GetDefault(1);
            RandomQuotes = TestData.GetRandom(1000);
            ZeroesQuotes = TestData.GetZeros();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Test data failed to load.");
            Console.WriteLine(ex);
            throw;
        }
    }
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

    // TODO: once caught up, make these abstract instead of virtual

    [TestMethod]
    public virtual void Standard()
        => Assert.Inconclusive("Test not implemented");

    [TestMethod]
    public virtual void BadData()
        => Assert.Inconclusive("Test not implemented");

    [TestMethod]
    public virtual void NoQuotes()
        => Assert.Inconclusive("Test not implemented");
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
