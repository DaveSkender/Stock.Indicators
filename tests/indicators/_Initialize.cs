using System.Globalization;
using System.Runtime.CompilerServices;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Other")]
[assembly: InternalsVisibleTo("Tests.Performance")]
namespace Tests.Common;

[TestClass]
public abstract class TestBase
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
