using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Kvo catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class KvoTests : TestBase
{
    [TestMethod]
    public void KvoSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Kvo.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<KvoResult> catalogResults = listing.Execute<KvoResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<KvoResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToKvo();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToKvo(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToKvo(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToKvo(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Kvo).GetMethod("ToKvo", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToKvo should exist");
            directResults = (IReadOnlyList<KvoResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
