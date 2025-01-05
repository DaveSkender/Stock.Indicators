using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

// GLOBALS & INITIALIZATION OF TEST DATA

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Other")]
[assembly: InternalsVisibleTo("Tests.Performance")]
namespace Tests.Common;

/// <summary>
/// Base class for all test classes, providing common test data and utilities.
/// </summary>
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

/// <summary>
/// Test class for the startup of the test project.
/// </summary>
public static class Startup
{
    /// <summary>
    /// Displays the assembly location, name, version, and target framework
    /// as a sanity check for test runner targeting.
    /// </summary>
    [AssemblyInitialize]
    public static void ShowFramework(TestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Get the assembly of any type from your Indicators project
        Assembly assembly = typeof(Indicator).Assembly;

        context.WriteLine($"Assembly Location: {assembly.Location}");
        context.WriteLine($"Assembly Name: {assembly.GetName().Name}");
        context.WriteLine($"Assembly Version: {assembly.GetName().Version}");

        // Get the target framework the assembly was built for
        TargetFrameworkAttribute targetFrameworkAttribute = assembly
            .GetCustomAttribute<TargetFrameworkAttribute>();

        context.WriteLine($"Target Framework: {targetFrameworkAttribute?.FrameworkName}");
    }
}
