using System.Globalization;

namespace Test.Base;

/// <summary>
/// base for all tests
/// </summary>
public abstract class TestBase
{
    internal static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    internal static readonly IReadOnlyList<Quote> Quotes = Test.Data.Data.GetDefault();
    internal static readonly IReadOnlyList<Quote> Intraday = Test.Data.Data.GetIntraday();
    internal static readonly IReadOnlyList<Quote> OtherQuotes = Test.Data.Data.GetCompare();
    internal static readonly IReadOnlyList<Quote> BadQuotes = Test.Data.Data.GetBad();
    internal static readonly IReadOnlyList<Quote> BigQuotes = Test.Data.Data.GetTooBig();
    internal static readonly IReadOnlyList<Quote> LongishQuotes = Test.Data.Data.GetLongish();
    internal static readonly IReadOnlyList<Quote> LongestQuotes = Test.Data.Data.GetLongest();
    internal static readonly IReadOnlyList<Quote> MismatchQuotes = Test.Data.Data.GetMismatch();
    internal static readonly IReadOnlyList<Quote> Noquotes = [];
    internal static readonly IReadOnlyList<Quote> Onequote = Test.Data.Data.GetDefault(1);
    internal static readonly IReadOnlyList<Quote> RandomQuotes = Test.Data.Data.GetRandom(1000);
    internal static readonly IReadOnlyList<Quote> ZeroesQuotes = Test.Data.Data.GetZeros();

    protected static readonly DateTime EvalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture);
}
