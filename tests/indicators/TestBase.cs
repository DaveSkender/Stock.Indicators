using System.Globalization;
using System.Runtime.CompilerServices;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Other")]
[assembly: InternalsVisibleTo("Tests.Performance")]
[assembly: InternalsVisibleTo("Observe.Streaming")]
namespace Tests.Common;

[TestClass]
public abstract class TestQuoteBase
{
    internal static readonly CultureInfo EnglishCulture = new("en-US", false);

    internal static readonly IEnumerable<Quote> quotes = TestData.GetDefault();
    internal static readonly IEnumerable<Quote> otherQuotes = TestData.GetCompare();
    internal static readonly IEnumerable<Quote> badQuotes = TestData.GetBad();
    internal static readonly IEnumerable<Quote> bigQuotes = TestData.GetTooBig();
    internal static readonly IEnumerable<Quote> maxQuotes = TestData.GetMax();
    internal static readonly IEnumerable<Quote> longishQuotes = TestData.GetLongish();
    internal static readonly IEnumerable<Quote> longestQuotes = TestData.GetLongest();
    internal static readonly IEnumerable<Quote> mismatchQuotes = TestData.GetMismatch();
    internal static readonly IEnumerable<Quote> noquotes = new List<Quote>();
    internal static readonly IEnumerable<Quote> onequote = TestData.GetDefault(1);
    internal static readonly IEnumerable<Quote> randomQuotes = TestData.GetRandom(1000);
    internal static readonly IEnumerable<Quote> zeroesQuotes = TestData.GetZeros();
    internal static readonly IEnumerable<(DateTime, double)> tupleNanny = TestData.GetTupleNaN();
}

/// <summary>
/// Base tests that all static indicators (series) should have.
/// </summary>
[TestClass]
public abstract class SeriesTestBase : TestQuoteBase
{
    internal readonly DateTime evalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);

    // TODO: once caught up, make these abstract instead of virtual

    [TestMethod]
    public virtual void Standard()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public virtual void BadData()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public virtual void NoQuotes()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public virtual void Equality()
    {
        Assert.Inconclusive();
    }
}

/// <summary>
/// Base tests that all streamed indicators should have.
/// </summary>
[TestClass]
public abstract class StreamTestBase : TestQuoteBase
{
    [TestMethod]
    public virtual void QuoteObserver()
    {
        Assert.Inconclusive();
    }

    [TestMethod]
    public virtual void Duplicates()
    {
        Assert.Inconclusive();
    }
}

/// <summary>
/// Additional tests all stream chainee indicators should have.
/// </summary>
public interface ITestChainObserver
{
    [TestMethod]
    void Chainee();
}

/// <summary>
/// Additional tests all stream chainor indicators should have.
/// </summary>
public interface ITestChainProvider
{
    [TestMethod]
    void Chainor();
}
