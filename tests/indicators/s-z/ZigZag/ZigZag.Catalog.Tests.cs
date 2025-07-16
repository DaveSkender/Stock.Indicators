using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for ZigZag catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ZigZagTests : TestBase
{
    [TestMethod]
    public void ZigZagSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = ZigZag.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ZigZagResult> catalogResults = listing.Execute<ZigZagResult>(quotes);

        // Get default parameters from catalog and call directly
        var endTypeParam = listing.Parameters.First(p => p.ParameterName == "endType");
        var percentChangeParam = listing.Parameters.First(p => p.ParameterName == "percentChange");
        
        EndType endType = (EndType)endTypeParam.DefaultValue!;
        decimal percentChange = (decimal)percentChangeParam.DefaultValue!;

        // Act - Direct call for comparison
        IReadOnlyList<ZigZagResult> directResults = quotes.ToZigZag(endType, percentChange);

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
