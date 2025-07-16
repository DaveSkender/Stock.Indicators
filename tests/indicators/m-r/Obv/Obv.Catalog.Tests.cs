using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Obv catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ObvTests : TestBase
{
    [TestMethod]
    public void ObvSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Obv.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ObvResult> catalogResults = listing.Execute<ObvResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<ObvResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToObv();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToObv(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToObv(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToObv(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Obv).GetMethod("ToObv", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToObv should exist");
            directResults = (IReadOnlyList<ObvResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
