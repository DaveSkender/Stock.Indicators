using System.Globalization;
using TestData = Test.Data.Data;

namespace Test.Base;

/// <summary>
/// base for all tests
/// </summary>
public abstract class TestBase
{
    internal static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    internal static readonly IReadOnlyList<Quote> Quotes = TestData.GetDefault();
    internal static readonly IReadOnlyList<Quote> Intraday = TestData.GetIntraday();
    internal static readonly IReadOnlyList<Quote> OtherQuotes = TestData.GetCompare();
    internal static readonly IReadOnlyList<Quote> BadQuotes = TestData.GetBad();
    internal static readonly IReadOnlyList<Quote> BigQuotes = TestData.GetTooBig();
    internal static readonly IReadOnlyList<Quote> LongishQuotes = TestData.GetLongish();
    internal static readonly IReadOnlyList<Quote> LongestQuotes = TestData.GetLongest();
    internal static readonly IReadOnlyList<Quote> MismatchQuotes = TestData.GetMismatch();
    internal static readonly IReadOnlyList<Quote> Noquotes = [];
    internal static readonly IReadOnlyList<Quote> Onequote = TestData.GetDefault(1);
    internal static readonly IReadOnlyList<Quote> RandomQuotes = TestData.GetRandom(1000);
    internal static readonly IReadOnlyList<Quote> ZeroesQuotes = TestData.GetZeros();

    internal const int quotesCount = 502;

    protected static readonly DateTime EvalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture);
}
