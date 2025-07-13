using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Catalog;

[TestClass]
public class CatalogStyleCounts : TestBase
{
    [TestMethod]
    public void CatalogShouldHaveCorrectStyleCounts()
    {
        // Arrange & Act
        var catalog = IndicatorCatalog.Catalog;
        
        var seriesCount = catalog.Count(x => x.Style == Style.Series);
        var streamCount = catalog.Count(x => x.Style == Style.Stream);
        var bufferCount = catalog.Count(x => x.Style == Style.Buffer);
        
        // Assert
        // Based on comment: "more than 80 series types, 2 for buffer, 6+ for hub types"
        // Note: "hub types" likely refers to Stream style in the current codebase
        seriesCount.Should().BeGreaterThan(80, "there should be more than 80 series style indicators");
        bufferCount.Should().BeGreaterOrEqualTo(2, "there should be at least 2 buffer style indicators");
        streamCount.Should().BeGreaterOrEqualTo(6, "there should be at least 6 stream style indicators (hub types)");
        
        // Additional verification - total should be reasonable
        var totalCount = seriesCount + streamCount + bufferCount;
        totalCount.Should().BeGreaterOrEqualTo(90, "total indicators should be at least 90");
        
        // Output for debugging purposes if test fails
        Console.WriteLine($"Catalog Style Counts: Series={seriesCount}, Stream={streamCount}, Buffer={bufferCount}, Total={totalCount}");
    }
}