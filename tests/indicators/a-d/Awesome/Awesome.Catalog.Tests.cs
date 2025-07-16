using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Awesome catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class AwesomeTests : TestBase
{
    [TestMethod]
    public void AwesomeSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Awesome.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<AwesomeResult> catalogResults = listing.Execute<AwesomeResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<AwesomeResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToAwesome();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToAwesome(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToAwesome(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToAwesome(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Awesome).GetMethod("ToAwesome", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToAwesome should exist");
            directResults = (IReadOnlyList<AwesomeResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
