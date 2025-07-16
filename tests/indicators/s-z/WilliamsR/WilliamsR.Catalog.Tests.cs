using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for WilliamsR catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class WilliamsRTests : TestBase
{
    [TestMethod]
    public void WilliamsRSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = WilliamsR.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<WilliamsResult> catalogResults = listing.Execute<WilliamsResult>(quotes);

        // Get default parameter from catalog and call directly
        var lookbackParam = listing.Parameters.First(p => p.ParameterName == "lookbackPeriods");
        int lookbackPeriods = (int)lookbackParam.DefaultValue!;

        // Act - Direct call for comparison
        IReadOnlyList<WilliamsResult> directResults = quotes.ToWilliamsR(lookbackPeriods);

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
