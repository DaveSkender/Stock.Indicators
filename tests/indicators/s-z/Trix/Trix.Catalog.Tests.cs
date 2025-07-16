using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Trix catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class TrixTests : TestBase
{
    [TestMethod]
    public void TrixSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Trix.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<TrixResult> catalogResults = listing.Execute<TrixResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<TrixResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToTrix();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToTrix(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToTrix(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToTrix(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Trix).GetMethod("ToTrix", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToTrix should exist");
            directResults = (IReadOnlyList<TrixResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
