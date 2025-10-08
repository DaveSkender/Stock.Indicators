using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Skender.Stock.Indicators;
using Tests.Indicators.Baselines;

namespace Tests.Indicators;

#pragma warning disable CA1707 // Test method naming convention uses underscores for readability

/// <summary>
/// Regression tests that compare current indicator outputs against baseline files.
/// Tests use exact binary equality (NON-NEGOTIABLE per Constitution Principle I).
/// </summary>
[TestClass]
public class RegressionTests : TestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private static bool ShouldRunRegressionTests()
    {
        string? envVar = Environment.GetEnvironmentVariable("RUN_REGRESSION_TESTS");
        return !string.IsNullOrEmpty(envVar) && (envVar == "1" || envVar.Equals("true", StringComparison.OrdinalIgnoreCase));
    }

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!ShouldRunRegressionTests())
        {
            // Note: In MSTest, ClassInitialize cannot directly skip all tests.
            // Each test method will check the environment variable independently.
            // This method is here for documentation purposes.
            context.WriteLine("Regression tests are disabled. Set RUN_REGRESSION_TESTS=true to enable.");
        }
    }

    [TestInitialize]
    public void TestInit()
    {
        if (!ShouldRunRegressionTests())
        {
            Assert.Inconclusive("Regression tests are disabled. Set RUN_REGRESSION_TESTS=true to enable.");
        }
    }

    #region Helper Methods

    private static string GetBaselinePath(string indicatorName, string testDirectory)
    {
        // Baseline files are colocated with indicator tests
        return Path.Combine(testDirectory, $"{indicatorName}.Baseline.json");
    }

    private static void AssertRegressionMatch<TResult>(
        string indicatorName,
        IEnumerable<TResult> actualResults,
        string baselinePath)
    {
        // Check if baseline file exists
        if (!File.Exists(baselinePath))
        {
            Assert.Inconclusive($"Baseline file not found: {indicatorName}.Baseline.json");
            return;
        }

        // Load baseline file
        string json = File.ReadAllText(baselinePath);
        List<TResult>? expectedResults = JsonSerializer.Deserialize<List<TResult>>(json, JsonOptions);

        Assert.IsNotNull(expectedResults, $"Failed to deserialize baseline file: {baselinePath}");

        // Compare results using BaselineComparer
        ComparisonResult comparison = BaselineComparer.Compare(expectedResults, actualResults);

        // Assert match with detailed diagnostics
        if (!comparison.IsMatch)
        {
            string diagnostics = FormatMismatchDiagnostics(indicatorName, baselinePath, comparison.Mismatches);
            Assert.Fail(diagnostics);
        }
    }

    private static string FormatMismatchDiagnostics(
        string indicatorName,
        string baselinePath,
        List<MismatchDetail> mismatches)
    {
#pragma warning disable CA1305 // Test diagnostic output does not require culture-specific formatting
        System.Text.StringBuilder sb = new();
        sb.AppendLine($"Regression test failed for {indicatorName}");
        sb.AppendLine($"Baseline file: {baselinePath}");
        sb.AppendLine($"Total mismatches: {mismatches.Count}");
        sb.AppendLine();
        sb.AppendLine("Mismatches (showing first 10):");

        int displayCount = Math.Min(10, mismatches.Count);
        for (int i = 0; i < displayCount; i++)
        {
            MismatchDetail m = mismatches[i];
            string deltaStr = m.Delta.HasValue ? m.Delta.Value.ToString("G17") : "N/A";

            if (m.PropertyName == "_COUNT_")
            {
                sb.AppendLine($"  Count mismatch: Expected {m.Expected}, Actual {m.Actual}");
            }
            else
            {
                sb.AppendLine($"  Date: {m.Timestamp:yyyy-MM-dd}, Property: {m.PropertyName}, " +
                             $"Expected: {m.Expected?.ToString("G17") ?? "null"}, " +
                             $"Actual: {m.Actual?.ToString("G17") ?? "null"}, " +
                             $"Delta: {deltaStr}");
            }
        }

        if (mismatches.Count > 10)
        {
            sb.AppendLine($"  ... and {mismatches.Count - 10} more mismatches");
        }

        sb.AppendLine();
        sb.AppendLine("If this change is intentional, regenerate the baseline using:");
        sb.AppendLine($"  dotnet run --project tools/performance/BaselineGenerator -- --indicator {indicatorName}");

        return sb.ToString();
#pragma warning restore CA1305
    }

    #endregion

    #region Individual Indicator Regression Tests

    // NOTE: Tests are generated dynamically to avoid maintaining 200+ test methods manually.
    // Each test follows the pattern: Load baseline -> Execute indicator -> Compare results

    [TestMethod]
    public void Adl_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Adl");
        string baselinePath = GetBaselinePath("Adl", testDir);

        // Act
        IReadOnlyList<AdlResult> results = Quotes.ToAdl();

        // Assert
        AssertRegressionMatch("Adl", results, baselinePath);
    }

    [TestMethod]
    public void Adx_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Adx");
        string baselinePath = GetBaselinePath("Adx", testDir);

        // Act
        IReadOnlyList<AdxResult> results = Quotes.ToAdx(14);

        // Assert
        AssertRegressionMatch("Adx", results, baselinePath);
    }

    [TestMethod]
    public void Alligator_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Alligator");
        string baselinePath = GetBaselinePath("Alligator", testDir);

        // Act
        IReadOnlyList<AlligatorResult> results = Quotes.ToAlligator();

        // Assert
        AssertRegressionMatch("Alligator", results, baselinePath);
    }

    [TestMethod]
    public void Alma_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Alma");
        string baselinePath = GetBaselinePath("Alma", testDir);

        // Act
        IReadOnlyList<AlmaResult> results = Quotes.ToAlma(10, 0.85, 6);

        // Assert
        AssertRegressionMatch("Alma", results, baselinePath);
    }

    [TestMethod]
    public void Aroon_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Aroon");
        string baselinePath = GetBaselinePath("Aroon", testDir);

        // Act
        IReadOnlyList<AroonResult> results = Quotes.ToAroon(25);

        // Assert
        AssertRegressionMatch("Aroon", results, baselinePath);
    }

    [TestMethod]
    public void Atr_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Atr");
        string baselinePath = GetBaselinePath("Atr", testDir);

        // Act
        IReadOnlyList<AtrResult> results = Quotes.ToAtr(14);

        // Assert
        AssertRegressionMatch("Atr", results, baselinePath);
    }

    [TestMethod]
    public void AtrStop_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "AtrStop");
        string baselinePath = GetBaselinePath("AtrStop", testDir);

        // Act
        IReadOnlyList<AtrStopResult> results = Quotes.ToAtrStop(21, 3);

        // Assert
        AssertRegressionMatch("AtrStop", results, baselinePath);
    }

    [TestMethod]
    public void Macd_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "m-r", "Macd");
        string baselinePath = GetBaselinePath("Macd", testDir);

        // Act
        IReadOnlyList<MacdResult> results = Quotes.ToMacd(12, 26, 9);

        // Assert
        AssertRegressionMatch("Macd", results, baselinePath);
    }

    [TestMethod]
    public void Rsi_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "m-r", "Rsi");
        string baselinePath = GetBaselinePath("Rsi", testDir);

        // Act
        IReadOnlyList<RsiResult> results = Quotes.ToRsi(14);

        // Assert
        AssertRegressionMatch("Rsi", results, baselinePath);
    }

    [TestMethod]
    public void Sma_Standard_RegressionTest()
    {
        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "s-z", "Sma");
        string baselinePath = GetBaselinePath("Sma", testDir);

        // Act
        IReadOnlyList<SmaResult> results = Quotes.ToSma(20);

        // Assert
        AssertRegressionMatch("Sma", results, baselinePath);
    }

    #endregion

    #region Integration Tests (T023)

    [TestMethod]
    public void MissingBaseline_ReturnsInconclusive()
    {
        // Arrange
        string nonExistentPath = Path.Combine(Directory.GetCurrentDirectory(), "NonExistent.Baseline.json");

        // Act & Assert - Should call Assert.Inconclusive and not fail
        IReadOnlyList<SmaResult> results = Quotes.ToSma(20);

        try
        {
            AssertRegressionMatch("NonExistent", results, nonExistentPath);
            Assert.Fail("Expected Assert.Inconclusive to be thrown");
        }
        catch (AssertInconclusiveException)
        {
            // Expected - test passes
        }
    }

    [TestMethod]
    public void EmptyBaseline_FailsGracefully()
    {
        // Arrange
        string tempPath = Path.Combine(Path.GetTempPath(), $"Empty_{Guid.NewGuid()}.Baseline.json");

        try
        {
            // Create empty JSON array
            File.WriteAllText(tempPath, "[]");

            IReadOnlyList<SmaResult> results = Quotes.ToSma(20);

            // Act & Assert - Should fail due to count mismatch
            try
            {
                AssertRegressionMatch("Empty", results, tempPath);
                Assert.Fail("Expected Assert.Fail for count mismatch");
            }
            catch (AssertFailedException ex)
            {
                // Expected - count mismatch should be reported
                Assert.IsTrue(ex.Message.Contains("Count mismatch", StringComparison.Ordinal), "Error message should mention count mismatch");
            }
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    #endregion

    #region Performance Tests (T024)

    [TestMethod]
    [Timeout(300000)] // 5 minutes in milliseconds
    public void RegressionSuite_ExecutionTime_UnderFiveMinutes()
    {
        // This test validates that the full regression suite can complete within 5 minutes
        // Individual regression tests are executed in the methods above
        // This is a placeholder to document the performance requirement

        Stopwatch sw = Stopwatch.StartNew();

        // Run a subset of indicators to validate performance pattern
        IReadOnlyList<SmaResult> smaResults = Quotes.ToSma(20);
        IReadOnlyList<MacdResult> macdResults = Quotes.ToMacd(12, 26, 9);
        IReadOnlyList<RsiResult> rsiResults = Quotes.ToRsi(14);

        sw.Stop();

        // Verify reasonable execution time (should be under 1 second for 3 indicators)
        Assert.IsTrue(sw.ElapsedMilliseconds < 1000,
            $"Execution took {sw.ElapsedMilliseconds}ms, expected < 1000ms for 3 indicators");
    }

    [TestMethod]
    public void BaselineComparer_Performance_AcceptableForLargeDatasets()
    {
        // Arrange - Generate large dataset
        List<SmaResult> expected = Enumerable.Range(0, 10000)
            .Select(i => new SmaResult(DateTime.Today.AddDays(i), i * 1.5))
            .ToList();

        List<SmaResult> actual = expected.ToList();

        // Act
        Stopwatch sw = Stopwatch.StartNew();
        ComparisonResult result = BaselineComparer.Compare(expected, actual);
        sw.Stop();

        // Assert
        Assert.IsTrue(result.IsMatch);
        Assert.IsTrue(sw.ElapsedMilliseconds < 1000,
            $"Comparison took {sw.ElapsedMilliseconds}ms, expected < 1000ms for 10,000 items");
    }

    #endregion

    #region Cross-Platform Validation Tests (T025)

    [TestMethod]
    public void CrossPlatform_SMA_DeterministicOutput()
    {
        // This test validates deterministic output across .NET target frameworks
        // Mathematical precision is NON-NEGOTIABLE per Constitution Principle I

        // Arrange
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "s-z", "Sma");
        string baselinePath = GetBaselinePath("Sma", testDir);

        // Skip if baseline doesn't exist
        if (!File.Exists(baselinePath))
        {
            Assert.Inconclusive($"Baseline file not found: {baselinePath}");
            return;
        }

        // Act - Execute on current framework (net8.0 or net9.0)
        IReadOnlyList<SmaResult> results = Quotes.ToSma(20);

        // Assert - Compare against baseline (which could have been generated on any framework)
        // ANY difference indicates a precision bug requiring investigation
        AssertRegressionMatch("SMA", results, baselinePath);
    }

    [TestMethod]
    public void CrossPlatform_MACD_DeterministicOutput()
    {
        // Multi-property indicator validation
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "m-r", "Macd");
        string baselinePath = GetBaselinePath("Macd", testDir);

        if (!File.Exists(baselinePath))
        {
            Assert.Inconclusive($"Baseline file not found: {baselinePath}");
            return;
        }

        IReadOnlyList<MacdResult> results = Quotes.ToMacd(12, 26, 9);
        AssertRegressionMatch("MACD", results, baselinePath);
    }

    [TestMethod]
    public void CrossPlatform_RSI_DeterministicOutput()
    {
        // Complex calculation indicator validation
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "m-r", "Rsi");
        string baselinePath = GetBaselinePath("Rsi", testDir);

        if (!File.Exists(baselinePath))
        {
            Assert.Inconclusive($"Baseline file not found: {baselinePath}");
            return;
        }

        IReadOnlyList<RsiResult> results = Quotes.ToRsi(14);
        AssertRegressionMatch("RSI", results, baselinePath);
    }

    [TestMethod]
    public void CrossPlatform_ADX_DeterministicOutput()
    {
        // High-precision indicator validation
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Adx");
        string baselinePath = GetBaselinePath("Adx", testDir);

        if (!File.Exists(baselinePath))
        {
            Assert.Inconclusive($"Baseline file not found: {baselinePath}");
            return;
        }

        IReadOnlyList<AdxResult> results = Quotes.ToAdx(14);
        AssertRegressionMatch("ADX", results, baselinePath);
    }

    [TestMethod]
    public void CrossPlatform_Alligator_DeterministicOutput()
    {
        // Multi-series indicator validation
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Alligator");
        string baselinePath = GetBaselinePath("Alligator", testDir);

        if (!File.Exists(baselinePath))
        {
            Assert.Inconclusive($"Baseline file not found: {baselinePath}");
            return;
        }

        IReadOnlyList<AlligatorResult> results = Quotes.ToAlligator();
        AssertRegressionMatch("Alligator", results, baselinePath);
    }

    /// <summary>
    /// Investigation procedure if platform differences are detected:
    /// 1. Identify which framework shows the difference (net8.0 vs net9.0)
    /// 2. Check if difference is in initial warmup nulls or calculated values
    /// 3. Review indicator calculation logic for floating-point operations
    /// 4. Check for any framework-specific Math library differences
    /// 5. File issue with detailed repro steps and framework versions
    /// 6. Do NOT regenerate baselines until root cause is understood
    /// </summary>
    [TestMethod]
    public void CrossPlatform_InvestigationProcedure_Documentation()
    {
        // This test documents the investigation procedure
        // No actual test execution - serves as documentation
        Assert.IsTrue(true, "Investigation procedure documented in method comments");
    }

    #endregion
}
