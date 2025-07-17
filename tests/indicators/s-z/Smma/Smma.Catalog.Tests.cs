using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Smma catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class SmmaTests : TestBase
{
    [TestMethod]
    public void SmmaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Smma.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<SmmaResult> catalogResults = listing.Execute<SmmaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<SmmaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToSmma();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToSmma((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToSmma(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToSmma(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Smma).GetMethod("ToSmma", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToSmma should exist");
            directResults = (IReadOnlyList<SmmaResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
