namespace Skender.Stock.Indicators.Tests;

// This class contains a method that should trigger our IND001 analyzer warning
public static class TestIndicator
{
    // This method will trigger IND001 because it's missing the required IndicatorAttribute
    public static IReadOnlyList<EmaResult> ToTestIndicator(this IReadOnlyList<Quote> quotes, int period) =>
        // This is just a simple test method to trigger our analyzer
        quotes.ToEma(period);
}

// Tests for the analyzer itself
[TestClass]
public class IndicatorAttributeAnalyzerTests
{
    [TestMethod]
    public void TestAnalyzer() =>
        // This test is just a placeholder. The actual test is whether
        // the analyzer flags the TestIndicator.ToTestIndicator method correctly.
        // When building the project, you should see a IND001 warning for that method.
        Assert.IsTrue(true);
}
