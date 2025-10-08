using System.Text.Json;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

#pragma warning disable CA1707 // Test method naming convention uses underscores for readability

/// <summary>
/// Regression tests that compare current indicator outputs against baseline files.
/// Uses FluentAssertions for exact equality comparison.
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
        string envVar = Environment.GetEnvironmentVariable("RUN_REGRESSION_TESTS");
        return !string.IsNullOrEmpty(envVar) && (envVar == "1" || envVar.Equals("true", StringComparison.OrdinalIgnoreCase));
    }

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!ShouldRunRegressionTests())
        {
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
        where TResult : ISeries
    {
        // Check if baseline file exists
        if (!File.Exists(baselinePath))
        {
            Assert.Inconclusive($"Baseline file not found: {indicatorName}.Baseline.json");
            return;
        }

        // Load and deserialize baseline file
        string json = File.ReadAllText(baselinePath);
        List<TResult> expectedResults = JsonSerializer.Deserialize<List<TResult>>(json, JsonOptions);

        Assert.IsNotNull(expectedResults, $"Failed to deserialize baseline file: {baselinePath}");

        // Compare using FluentAssertions with exact equality
        actualResults.Should().BeEquivalentTo(expectedResults, options => options
            .WithStrictOrdering()
            .ComparingByMembers<TResult>()
            .WithTracing(), // Provides detailed diagnostics on mismatch
            $"Regression test failed for {indicatorName}. " +
            $"If this change is intentional, regenerate the baseline using: " +
            $"dotnet run --project tools/performance/BaselineGenerator -- --indicator {indicatorName}");
    }

    #endregion

    #region Individual Indicator Regression Tests

    [TestMethod]
    public void Adl_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Adl");
        string baselinePath = GetBaselinePath("Adl", testDir);

        IReadOnlyList<AdlResult> results = Quotes.ToAdl();

        AssertRegressionMatch("Adl", results, baselinePath);
    }

    [TestMethod]
    public void Adx_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Adx");
        string baselinePath = GetBaselinePath("Adx", testDir);

        IReadOnlyList<AdxResult> results = Quotes.ToAdx(14);

        AssertRegressionMatch("Adx", results, baselinePath);
    }

    [TestMethod]
    public void Alligator_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Alligator");
        string baselinePath = GetBaselinePath("Alligator", testDir);

        IReadOnlyList<AlligatorResult> results = Quotes.ToAlligator();

        AssertRegressionMatch("Alligator", results, baselinePath);
    }

    [TestMethod]
    public void Alma_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Alma");
        string baselinePath = GetBaselinePath("Alma", testDir);

        IReadOnlyList<AlmaResult> results = Quotes.ToAlma(10, 0.85, 6);

        AssertRegressionMatch("Alma", results, baselinePath);
    }

    [TestMethod]
    public void Aroon_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Aroon");
        string baselinePath = GetBaselinePath("Aroon", testDir);

        IReadOnlyList<AroonResult> results = Quotes.ToAroon(25);

        AssertRegressionMatch("Aroon", results, baselinePath);
    }

    [TestMethod]
    public void Atr_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "Atr");
        string baselinePath = GetBaselinePath("Atr", testDir);

        IReadOnlyList<AtrResult> results = Quotes.ToAtr(14);

        AssertRegressionMatch("Atr", results, baselinePath);
    }

    [TestMethod]
    public void AtrStop_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "a-d", "AtrStop");
        string baselinePath = GetBaselinePath("AtrStop", testDir);

        IReadOnlyList<AtrStopResult> results = Quotes.ToAtrStop(21, 3);

        AssertRegressionMatch("AtrStop", results, baselinePath);
    }

    [TestMethod]
    public void Macd_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "m-r", "Macd");
        string baselinePath = GetBaselinePath("Macd", testDir);

        IReadOnlyList<MacdResult> results = Quotes.ToMacd(12, 26, 9);

        AssertRegressionMatch("Macd", results, baselinePath);
    }

    [TestMethod]
    public void Rsi_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "m-r", "Rsi");
        string baselinePath = GetBaselinePath("Rsi", testDir);

        IReadOnlyList<RsiResult> results = Quotes.ToRsi(14);

        AssertRegressionMatch("Rsi", results, baselinePath);
    }

    [TestMethod]
    public void Sma_Standard_RegressionTest()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "s-z", "Sma");
        string baselinePath = GetBaselinePath("Sma", testDir);

        IReadOnlyList<SmaResult> results = Quotes.ToSma(20);

        AssertRegressionMatch("Sma", results, baselinePath);
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public void MissingBaseline_ReturnsInconclusive()
    {
        string nonExistentPath = Path.Combine(Directory.GetCurrentDirectory(), "NonExistent.Baseline.json");

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
                Assert.Fail("Expected assertion failure for empty baseline");
            }
            catch (AssertFailedException)
            {
                // Expected - empty baseline should not match actual results
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

    #region Cross-Platform Validation Tests

    [TestMethod]
    public void CrossPlatform_SMA_DeterministicOutput()
    {
        string testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "s-z", "Sma");
        string baselinePath = GetBaselinePath("Sma", testDir);

        if (!File.Exists(baselinePath))
        {
            Assert.Inconclusive($"Baseline file not found: {baselinePath}");
            return;
        }

        IReadOnlyList<SmaResult> results = Quotes.ToSma(20);
        AssertRegressionMatch("SMA", results, baselinePath);
    }

    [TestMethod]
    public void CrossPlatform_MACD_DeterministicOutput()
    {
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

    #endregion
}
