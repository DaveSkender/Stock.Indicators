using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Doji catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class DojiTests : TestBase
{
    [TestMethod]
    public void DojiSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Doji.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<DojiResult> catalogResults = listing.Execute<DojiResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<DojiResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToDoji();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToDoji(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToDoji(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToDoji(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Doji).GetMethod("ToDoji", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToDoji should exist");
            directResults = (IReadOnlyList<DojiResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
