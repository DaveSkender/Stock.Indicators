using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Ichimoku catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class IchimokuTests : TestBase
{
    [TestMethod]
    public void IchimokuSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Ichimoku.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<IchimokuResult> catalogResults = listing.Execute<IchimokuResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<IchimokuResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToIchimoku();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToIchimoku(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToIchimoku(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToIchimoku(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Ichimoku).GetMethod("ToIchimoku", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToIchimoku should exist");
            directResults = (IReadOnlyList<IchimokuResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
