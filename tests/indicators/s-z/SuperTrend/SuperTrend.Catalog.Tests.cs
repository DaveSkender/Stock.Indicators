using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for SuperTrend catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class SuperTrendTests : TestBase
{
    [TestMethod]
    public void SuperTrendSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = SuperTrend.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<SuperTrendResult> catalogResults = listing.Execute<SuperTrendResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<SuperTrendResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToSuperTrend();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToSuperTrend(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToSuperTrend(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToSuperTrend(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(SuperTrend).GetMethod("ToSuperTrend", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToSuperTrend should exist");
            directResults = (IReadOnlyList<SuperTrendResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
