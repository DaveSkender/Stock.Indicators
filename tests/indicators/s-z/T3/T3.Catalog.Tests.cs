using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for T3 catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class T3Tests : TestBase
{
    [TestMethod]
    public void T3SeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = T3.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<T3Result> catalogResults = listing.Execute<T3Result>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<T3Result> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToT3();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToT3(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToT3(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToT3(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(T3).GetMethod("ToT3", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToT3 should exist");
            directResults = (IReadOnlyList<T3Result>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
