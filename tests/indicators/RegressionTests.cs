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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
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

    #region Integration Tests

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

    #endregion
}
