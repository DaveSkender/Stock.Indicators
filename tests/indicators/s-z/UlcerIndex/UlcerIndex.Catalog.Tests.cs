using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for UlcerIndex catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class UlcerIndexTests : TestBase
{
    [TestMethod]
    public void UlcerIndexSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = UlcerIndex.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<UlcerIndexResult> catalogResults = listing.Execute<UlcerIndexResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<UlcerIndexResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToUlcerIndex();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToUlcerIndex((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToUlcerIndex(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToUlcerIndex(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(UlcerIndex).GetMethod("ToUlcerIndex", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToUlcerIndex should exist");
            directResults = (IReadOnlyList<UlcerIndexResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
