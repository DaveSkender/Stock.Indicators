using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Dema catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class DemaTests : TestBase
{
    [TestMethod]
    public void DemaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Dema.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<DemaResult> catalogResults = listing.Execute<DemaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<DemaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToDema();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToDema(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToDema(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToDema(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Dema).GetMethod("ToDema", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToDema should exist");
            directResults = (IReadOnlyList<DemaResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
