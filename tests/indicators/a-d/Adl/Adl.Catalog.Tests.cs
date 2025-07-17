namespace Catalog;

/// <summary>
/// Test class for ADL catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class AdlTests : TestBase
{
    [TestMethod]
    public void AdlSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Adl.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<AdlResult> catalogResults = listing.Execute<AdlResult>(quotes);

        // Act - Direct call for comparison
        IReadOnlyList<AdlResult> directResults = quotes.ToAdl();

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
