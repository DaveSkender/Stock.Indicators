using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for HtTrendline catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class HtTrendlineTests : TestBase
{
    [TestMethod]
    public void HtTrendlineSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = HtTrendline.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<HtlResult> catalogResults = listing.Execute<HtlResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<HtlResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToHtTrendline();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToHtTrendline(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToHtTrendline(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToHtTrendline(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(HtTrendline).GetMethod("ToHtTrendline", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToHtTrendline should exist");
            directResults = (IReadOnlyList<HtlResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
