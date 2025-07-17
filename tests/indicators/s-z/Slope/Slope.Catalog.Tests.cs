using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Slope catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class SlopeTests : TestBase
{
    [TestMethod]
    public void SlopeSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Slope.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<SlopeResult> catalogResults = listing.Execute<SlopeResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<SlopeResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToSlope();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToSlope((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToSlope(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToSlope(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Slope).GetMethod("ToSlope", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToSlope should exist");
            directResults = (IReadOnlyList<SlopeResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
