using System.Globalization;

namespace Tests.Data;

/// <summary>
/// base for all tests
/// </summary>
public abstract class TestBase
{
    internal static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Precision constants for BeApproximately() assertions against manually calculated reference values
    /// Maps to equivalent .Round() precision expectations for Series-style indicators only
    /// </summary>
    internal const double Money3 = 0.0005;    // 3 decimal places: ±0.0005 (equivalent to .Round(3))
    /// <summary>
    /// 4 decimal places: &#177;0.00005 (equivalent to .Round(4))
    /// </summary>
    internal const double Money4 = 0.00005;
    /// <summary>
    /// 5 decimal places: &#177;0.000005 (equivalent to .Round(5))
    /// </summary>
    internal const double Money5 = 0.000005;
    /// <summary>
    /// 6 decimal places: &#177;0.0000005 (equivalent to .Round(6))
    /// </summary>
    internal const double Money6 = 0.0000005;

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

    protected static readonly DateTime EvalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", invariantCulture);
}
