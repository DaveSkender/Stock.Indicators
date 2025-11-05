using System.Reflection;

namespace Tests.Data;

/// <summary>
/// Validates that all StreamHub test classes implement correct test interfaces
/// based on their hub's provider type per instruction file guidelines.
/// </summary>
[TestClass]
public class StreamHubInterfaceComplianceTests
{
    private static readonly Dictionary<string, (Type hubType, Type[] expectedInterfaces)> _expectations = new() {
        // ChainProvider<IReusable, T> should have ITestChainObserver + ITestChainProvider
        ["EmaHubTests"] = (typeof(ITestChainObserver),
            [typeof(ITestChainObserver), typeof(ITestChainProvider)]),
        ["SmaHubTests"] = (typeof(ITestChainObserver),
            [typeof(ITestChainObserver), typeof(ITestChainProvider)]),
        ["RsiHubTests"] = (typeof(ITestChainObserver),
            [typeof(ITestChainObserver), typeof(ITestChainProvider)]),
        ["MacdHubTests"] = (typeof(ITestChainObserver),
            [typeof(ITestChainObserver), typeof(ITestChainProvider)]),

        // ChainProvider<IQuote, T> should have ITestQuoteObserver + ITestChainProvider
        ["AdxHubTests"] = (typeof(ITestQuoteObserver),
            [typeof(ITestQuoteObserver), typeof(ITestChainProvider)]),
        ["AtrHubTests"] = (typeof(ITestQuoteObserver),
            [typeof(ITestQuoteObserver), typeof(ITestChainProvider)]),
        ["CciHubTests"] = (typeof(ITestQuoteObserver),
            [typeof(ITestQuoteObserver), typeof(ITestChainProvider)]),
        ["BollingerBandsStreamHubTests"] = (typeof(ITestQuoteObserver),
            [typeof(ITestQuoteObserver), typeof(ITestChainProvider)]),

        // PairsProvider<T> should have only ITestPairsObserver
        ["BetaHub"] = (typeof(ITestPairsObserver),
            [typeof(ITestPairsObserver)]),
        ["CorrelationHub"] = (typeof(ITestPairsObserver),
            [typeof(ITestPairsObserver)]),
        ["PrsHub"] = (typeof(ITestPairsObserver),
            [typeof(ITestPairsObserver)]),

        // StreamHub<IQuote, T> should have only ITestQuoteObserver
        ["PivotsHubTests"] = (typeof(ITestQuoteObserver),
            [typeof(ITestQuoteObserver)]),
        ["ChandelierHubTests"] = (typeof(ITestQuoteObserver),
            [typeof(ITestQuoteObserver)]),

        // StreamHub<IReusable, T> should have only ITestChainObserver
        ["AlligatorHubTests"] = (typeof(ITestChainObserver),
            [typeof(ITestChainObserver)]),
        ["MaEnvelopesHubTests"] = (typeof(ITestChainObserver),
            [typeof(ITestChainObserver)]),
    };

    [TestMethod]
    public void AllStreamHubTests_ImplementCorrectInterfaces()
    {
        // Get all test classes that inherit from StreamHubTestBase
        Assembly testAssembly = typeof(StreamHubTestBase).Assembly;
        Type[] allTypes = testAssembly.GetTypes();

        List<Type> streamHubTestClasses = allTypes
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && typeof(StreamHubTestBase).IsAssignableFrom(t))
            .ToList();

        Console.WriteLine($"\nFound {streamHubTestClasses.Count} StreamHub test classes");

        List<string> violations = [];
        int validated = 0;

        foreach (Type testClass in streamHubTestClasses)
        {
            string className = testClass.Name;

            // Skip if we don't have expectations defined (not all indicators validated yet)
            if (!_expectations.ContainsKey(className))
                continue;

            validated++;
            (Type _, Type[] expectedInterfaces) = _expectations[className];
            Type[] actualInterfaces = testClass.GetInterfaces()
                .Where(i => i.Namespace == "Tests.Data"
                         && i.Name.StartsWith("ITest", StringComparison.Ordinal))
                .ToArray();

            // Check if all expected interfaces are implemented
            foreach (Type expectedInterface in expectedInterfaces)
            {
                if (!actualInterfaces.Contains(expectedInterface))
                {
                    violations.Add($"{className}: Missing {expectedInterface.Name}");
                }
            }

            // Check for unexpected interfaces (e.g., ITestChainProvider on non-chainable hubs)
            if (expectedInterfaces.Length == 1 && actualInterfaces.Length > 1)
            {
                foreach (Type actualInterface in actualInterfaces)
                {
                    if (!expectedInterfaces.Contains(actualInterface)
                        && actualInterface.Name != "ITestQuoteObserver")
                    {
                        // ITestChainObserver inherits ITestQuoteObserver
                        violations.Add($"{className}: Unexpected {actualInterface.Name}");
                    }
                }
            }
        }

        Console.WriteLine($"Validated {validated} test classes");

        if (violations.Count > 0)
        {
            string message = $"Found {violations.Count} interface compliance violations:\n"
                           + string.Join("\n", violations);
            Assert.Fail(message);
        }

        Console.WriteLine("✅ All validated StreamHub tests implement correct interfaces");
    }

    [TestMethod]
    public void ChainObserver_DoesNotRedundantlyImplementQuoteObserver()
    {
        // ITestChainObserver inherits ITestQuoteObserver, so implementing both is redundant

        Assembly testAssembly = typeof(StreamHubTestBase).Assembly;
        Type[] allTypes = testAssembly.GetTypes();

        List<Type> redundantImplementations = allTypes
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && typeof(ITestChainObserver).IsAssignableFrom(t)
                     && t.GetInterfaces().Contains(typeof(ITestQuoteObserver)))
            .ToList();

        // Note: This is informational only - it's not wrong to explicitly implement both,
        // but the instruction file notes it's redundant
        if (redundantImplementations.Count > 0)
        {
            Console.WriteLine($"\nNote: {redundantImplementations.Count} test classes "
                            + "explicitly implement both ITestChainObserver and ITestQuoteObserver "
                            + "(ITestChainObserver already inherits ITestQuoteObserver):");

            foreach (Type t in redundantImplementations)
            {
                Console.WriteLine($"  - {t.Name}");
            }
        }
    }

    [TestMethod]
    public void PairsObserver_DoesNotImplementQuoteObserver()
    {
        // PairsProvider tests should NOT implement ITestQuoteObserver or ITestChainObserver
        // They are dual-stream and have different synchronization requirements

        Assembly testAssembly = typeof(StreamHubTestBase).Assembly;
        Type[] allTypes = testAssembly.GetTypes();

        List<Type> violations = allTypes
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && typeof(ITestPairsObserver).IsAssignableFrom(t)
                     && (typeof(ITestQuoteObserver).IsAssignableFrom(t)
                      || typeof(ITestChainObserver).IsAssignableFrom(t)))
            .ToList();

        if (violations.Count > 0)
        {
            string message = $"Found {violations.Count} PairsObserver tests incorrectly "
                           + "implementing QuoteObserver or ChainObserver:\n"
                           + string.Join("\n", violations.Select(t => $"  - {t.Name}"));
            Assert.Fail(message);
        }

        Console.WriteLine("✅ All PairsObserver tests correctly avoid QuoteObserver/ChainObserver interfaces");
    }
}
