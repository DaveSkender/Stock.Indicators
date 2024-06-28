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
    internal static readonly CultureInfo EnglishCulture = new("en-US", false);

    internal static IEnumerable<Quote> quotes = [];
    internal static IEnumerable<Quote> otherQuotes = [];
    internal static IEnumerable<Quote> badQuotes = [];
    internal static IEnumerable<Quote> bigQuotes = [];
    internal static IEnumerable<Quote> maxQuotes = [];
    internal static IEnumerable<Quote> longishQuotes = [];
    internal static IEnumerable<Quote> longestQuotes = [];
    internal static IEnumerable<Quote> mismatchQuotes = [];
    internal static IEnumerable<Quote> noquotes = [];
    internal static IEnumerable<Quote> onequote = [];
    internal static IEnumerable<Quote> randomQuotes = [];
    internal static IEnumerable<Quote> zeroesQuotes = [];
    internal static IEnumerable<(DateTime, double)> tupleNanny = [];

    internal TestBase()
    {
        try
        {
            quotes = TestData.GetDefault();
            otherQuotes = TestData.GetCompare();
            badQuotes = TestData.GetBad();
            bigQuotes = TestData.GetTooBig();
            maxQuotes = TestData.GetMax();
            longishQuotes = TestData.GetLongish();
            longestQuotes = TestData.GetLongest();
            mismatchQuotes = TestData.GetMismatch();
            noquotes = new List<Quote>();
            onequote = TestData.GetDefault(1);
            randomQuotes = TestData.GetRandom(1000);
            zeroesQuotes = TestData.GetZeros();
            tupleNanny = TestData.GetTupleNaN();
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
    internal readonly DateTime evalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);

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
