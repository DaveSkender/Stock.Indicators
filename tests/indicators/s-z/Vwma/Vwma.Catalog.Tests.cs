using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Vwma catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class VwmaTests : TestBase
{
    [TestMethod]
    public void VwmaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Vwma.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<VwmaResult> catalogResults = listing.Execute<VwmaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<VwmaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToVwma();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToVwma((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToVwma(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToVwma(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Vwma).GetMethod("ToVwma", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToVwma should exist");
            directResults = (IReadOnlyList<VwmaResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
