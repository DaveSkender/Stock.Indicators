using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Donchian catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class DonchianTests : TestBase
{
    [TestMethod]
    public void DonchianSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Donchian.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<DonchianResult> catalogResults = listing.Execute<DonchianResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<DonchianResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToDonchian();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToDonchian((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToDonchian(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToDonchian(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Donchian).GetMethod("ToDonchian", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToDonchian should exist");
            directResults = (IReadOnlyList<DonchianResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
