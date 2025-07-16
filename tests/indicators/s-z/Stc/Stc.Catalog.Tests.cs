using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Stc catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class StcTests : TestBase
{
    [TestMethod]
    public void StcSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Stc.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<StcResult> catalogResults = listing.Execute<StcResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<StcResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToStc();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToStc(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToStc(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToStc(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Stc).GetMethod("ToStc", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToStc should exist");
            directResults = (IReadOnlyList<StcResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
