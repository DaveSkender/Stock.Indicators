using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Keltner catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class KeltnerTests : TestBase
{
    [TestMethod]
    public void KeltnerSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Keltner.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<KeltnerResult> catalogResults = listing.Execute<KeltnerResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<KeltnerResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToKeltner();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToKeltner(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToKeltner(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToKeltner(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Keltner).GetMethod("ToKeltner", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToKeltner should exist");
            directResults = (IReadOnlyList<KeltnerResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
