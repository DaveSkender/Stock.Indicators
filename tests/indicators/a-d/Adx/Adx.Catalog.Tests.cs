namespace Catalog;

/// <summary>
/// Test class for ADX catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class AdxTests : TestBase
{
    [TestMethod]
    public void AdxSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Adx.SeriesListing;
        listing.Should().NotBeNull();

        // Get default parameter values from catalog
        var lookbackParam = listing.Parameters.First(p => p.ParameterName == "lookbackPeriods");
        int lookbackPeriods = (int)lookbackParam.DefaultValue!;

        // Act - Call using catalog metadata
        IReadOnlyList<AdxResult> catalogResults = listing.Execute<AdxResult>(quotes);

        // Act - Direct call for comparison
        IReadOnlyList<AdxResult> directResults = quotes.ToAdx(lookbackPeriods);

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}