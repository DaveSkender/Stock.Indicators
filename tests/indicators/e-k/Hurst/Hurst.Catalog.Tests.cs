using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Hurst catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class HurstTests : TestBase
{
    [TestMethod]
    public void HurstSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Hurst.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<HurstResult> catalogResults = listing.Execute<HurstResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<HurstResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToHurst();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToHurst((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToHurst(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToHurst(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Hurst).GetMethod("ToHurst", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToHurst should exist");
            directResults = (IReadOnlyList<HurstResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
