using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for VolatilityStop catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class VolatilityStopTests : TestBase
{
    [TestMethod]
    public void VolatilityStopSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = VolatilityStop.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<VolatilityStopResult> catalogResults = listing.Execute<VolatilityStopResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<VolatilityStopResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToVolatilityStop();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToVolatilityStop((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToVolatilityStop((int)parameters[0], (double)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToVolatilityStop(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(VolatilityStop).GetMethod("ToVolatilityStop", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToVolatilityStop should exist");
            directResults = (IReadOnlyList<VolatilityStopResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
