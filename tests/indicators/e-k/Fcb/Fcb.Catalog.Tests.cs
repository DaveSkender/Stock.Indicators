using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Fcb catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class FcbTests : TestBase
{
    [TestMethod]
    public void FcbSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Fcb.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<FcbResult> catalogResults = listing.Execute<FcbResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<FcbResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToFcb();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToFcb(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToFcb(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToFcb(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Fcb).GetMethod("ToFcb", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToFcb should exist");
            directResults = (IReadOnlyList<FcbResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
