using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Vwap catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class VwapTests : TestBase
{
    [TestMethod]
    public void VwapSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Vwap.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<VwapResult> catalogResults = listing.Execute<VwapResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<VwapResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToVwap();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToVwap((DateTime)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToVwap(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToVwap(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Vwap).GetMethod("ToVwap", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToVwap should exist");
            directResults = (IReadOnlyList<VwapResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
