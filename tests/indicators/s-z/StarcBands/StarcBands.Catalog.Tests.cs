using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for StarcBands catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class StarcBandsTests : TestBase
{
    [TestMethod]
    public void StarcBandsSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = StarcBands.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<StarcBandsResult> catalogResults = listing.Execute<StarcBandsResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<StarcBandsResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToStarcBands();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToStarcBands((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToStarcBands((int)parameters[0], (double)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToStarcBands((int)parameters[0], (double)parameters[1], (int)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(StarcBands).GetMethod("ToStarcBands", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToStarcBands should exist");
            directResults = (IReadOnlyList<StarcBandsResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
