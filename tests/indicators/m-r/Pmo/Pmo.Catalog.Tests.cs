using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Pmo catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class PmoTests : TestBase
{
    [TestMethod]
    public void PmoSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Pmo.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<PmoResult> catalogResults = listing.Execute<PmoResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<PmoResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToPmo();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToPmo((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToPmo((int)parameters[0], (int)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToPmo((int)parameters[0], (int)parameters[1], (int)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Pmo).GetMethod("ToPmo", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToPmo should exist");
            directResults = (IReadOnlyList<PmoResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
