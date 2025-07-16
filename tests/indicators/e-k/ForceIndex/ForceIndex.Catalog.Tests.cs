using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for ForceIndex catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ForceIndexTests : TestBase
{
    [TestMethod]
    public void ForceIndexSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = ForceIndex.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ForceIndexResult> catalogResults = listing.Execute<ForceIndexResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<ForceIndexResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToForceIndex();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToForceIndex(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToForceIndex(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToForceIndex(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(ForceIndex).GetMethod("ToForceIndex", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToForceIndex should exist");
            directResults = (IReadOnlyList<ForceIndexResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
