#nullable enable
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Catalog;

/// <summary>
/// Tests that validate the catalog's ability to support automation scenarios.
/// These tests ensure that indicators can be called programmatically using catalog metadata.
/// </summary>
[TestClass]
public class CatalogAutomationTests : TestBase
{
    [TestMethod]
    public void AllIndicatorsShouldHaveMethodNamesForAutomation()
    {
        // Arrange & Act - Get all catalog entries
        var catalog = IndicatorCatalog.Catalog;

        // Assert - Every indicator must have a method name for automation
        catalog.Should().NotBeEmpty("catalog should contain indicator listings");

        foreach (var listing in catalog)
        {
            listing.MethodName.Should().NotBeNullOrWhiteSpace($"indicator {listing.Uiid} should have a method name for automation");
            listing.MethodName.Should().StartWith("To", $"method name for {listing.Uiid} should start with 'To'");
        }
    }

    [TestMethod]
    public void CatalogExecutionShouldWorkForKnownIndicators()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        var catalog = IndicatorCatalog.Catalog;
        
        // Test known working indicators that we know work well
        string[] testIndicators = ["EMA", "SMA", "RSI"];
        
        foreach (var uiid in testIndicators)
        {
            var listing = catalog.FirstOrDefault(l => l.Uiid == uiid && l.Style == Style.Series);
            if (listing == null) continue;

            try
            {
                // Act - Execute via catalog with default parameters
                var catalogResults = listing.Execute<IReusable>(quotes);
                
                // Assert - Should produce results without throwing
                catalogResults.Should().NotBeNull($"catalog execution should work for {listing.Uiid}");
                catalogResults.Should().NotBeEmpty($"catalog execution should produce results for {listing.Uiid}");
                
                // Verify results are valid
                catalogResults.Where(r => r != null).Should().NotBeEmpty($"catalog should produce valid results for {listing.Uiid}");
            }
            catch (Exception ex) when (ex is not AssertFailedException)
            {
                Assert.Fail($"Catalog execution failed for {listing.Uiid}: {ex.Message}");
            }
        }
    }

    [TestMethod]
    public void CatalogShouldSupportParameterTypeValidation()
    {
        // Arrange
        var emaListing = IndicatorCatalog.Catalog.First(l => l.Uiid == "EMA" && l.Style == Style.Series);
        
        // Act & Assert - Valid parameter type
        Action validAction = () => emaListing.WithParamValue("lookbackPeriods", 20);
        validAction.Should().NotThrow("valid parameter type should work");
        
        // Act & Assert - Invalid parameter type should throw
        Action invalidAction = () => emaListing.WithParamValue("lookbackPeriods", "invalid");
        invalidAction.Should().Throw<ArgumentException>("invalid parameter type should throw");
    }

    [TestMethod]
    public void CatalogStyleCountsShouldMatchExpected()
    {
        // Arrange & Act
        var catalog = IndicatorCatalog.Catalog;
        var styleCounts = catalog
            .GroupBy(l => l.Style)
            .ToDictionary(g => g.Key, g => g.Count());

        // Assert - Verify expected distribution
        styleCounts[Style.Series].Should().Be(84, "should have exactly 84 series indicators");
        styleCounts[Style.Stream].Should().Be(8, "should have exactly 8 stream indicators");  
        styleCounts[Style.Buffer].Should().Be(2, "should have exactly 2 buffer indicators");
        catalog.Count.Should().Be(94, "total catalog count should be 94");
    }

    [TestMethod]
    public void CatalogShouldHaveValidStructure()
    {
        // Arrange & Act - Get all catalog entries
        var catalog = IndicatorCatalog.Catalog;

        // Assert - Each entry should have valid structure
        catalog.Should().NotBeEmpty("catalog should contain indicator listings");

        foreach (var listing in catalog)
        {
            // Basic properties
            listing.Uiid.Should().NotBeNullOrWhiteSpace($"indicator should have a UIID");
            listing.Name.Should().NotBeNullOrWhiteSpace($"indicator {listing.Uiid} should have a name");
            listing.Style.Should().BeDefined($"indicator {listing.Uiid} should have a valid style");
            listing.Category.Should().BeDefined($"indicator {listing.Uiid} should have a valid category");

            // Results should be valid if present
            if (listing.Results != null && listing.Results.Count > 0)
            {
                foreach (var result in listing.Results)
                {
                    result.DataName.Should().NotBeNullOrWhiteSpace($"result for {listing.Uiid} should have a data name");
                    result.DisplayName.Should().NotBeNullOrWhiteSpace($"result for {listing.Uiid} should have a display name");
                }
            }
        }
    }
}