using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Hma catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class HmaTests : TestBase
{
    [TestMethod]
    public void HmaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Hma.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<HmaResult> catalogResults = listing.Execute<HmaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<HmaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToHma();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToHma((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToHma(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToHma(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Hma).GetMethod("ToHma", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToHma should exist");
            directResults = (IReadOnlyList<HmaResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
