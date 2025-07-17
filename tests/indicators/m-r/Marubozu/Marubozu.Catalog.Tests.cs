using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Marubozu catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class MarubozuTests : TestBase
{
    [TestMethod]
    public void MarubozuSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Marubozu.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<CandleResult> catalogResults = listing.Execute<CandleResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<CandleResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToMarubozu();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToMarubozu((double)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToMarubozu(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToMarubozu(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Marubozu).GetMethod("ToMarubozu", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToMarubozu should exist");
            directResults = (IReadOnlyList<CandleResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
