using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Gator catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class GatorTests : TestBase
{
    [TestMethod]
    public void GatorSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Gator.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<GatorResult> catalogResults = listing.Execute<GatorResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<GatorResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToGator();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToGator(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToGator(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToGator(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Gator).GetMethod("ToGator", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToGator should exist");
            directResults = (IReadOnlyList<GatorResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
