using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Bop catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class BopTests : TestBase
{
    [TestMethod]
    public void BopSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Bop.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<BopResult> catalogResults = listing.Execute<BopResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<BopResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToBop();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToBop(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToBop(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToBop(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Bop).GetMethod("ToBop", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToBop should exist");
            directResults = (IReadOnlyList<BopResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
