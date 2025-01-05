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
[TestClass]
public class Startup
{
    /// <summary>
    /// Displays the assembly location, name, version, and target framework
    /// as a sanity check for test runner targeting.
    /// </summary>
    [TestMethod]
    [TestCategory("ShowMe")]
    public void ShowFramework()
    {
        // Get the assembly of any type from your Indicators project
        Assembly assembly = typeof(Indicator).Assembly;

        Console.WriteLine($"Assembly Location: {assembly.Location}");
        Console.WriteLine($"Assembly Name: {assembly.GetName().Name}");
        Console.WriteLine($"Assembly Version: {assembly.GetName().Version}");

        // Get the target framework the assembly was built for
        TargetFrameworkAttribute targetFrameworkAttribute = assembly
            .GetCustomAttribute<TargetFrameworkAttribute>();

        string frameworkName
            = targetFrameworkAttribute?.FrameworkName ?? "Unknown";

        Console.WriteLine($"Target Framework: {frameworkName}");

        frameworkName.Should().NotBe("Unknown");
    }
}
