using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for StdDev catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class StdDevTests : TestBase
{
    [TestMethod]
    public void StdDevSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = StdDev.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<StdDevResult> catalogResults = listing.Execute<StdDevResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<StdDevResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToStdDev();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToStdDev((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToStdDev(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToStdDev(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(StdDev).GetMethod("ToStdDev", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToStdDev should exist");
            directResults = (IReadOnlyList<StdDevResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
