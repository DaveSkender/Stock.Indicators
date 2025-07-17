using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Kama catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class KamaTests : TestBase
{
    [TestMethod]
    public void KamaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Kama.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<KamaResult> catalogResults = listing.Execute<KamaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<KamaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToKama();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToKama((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToKama((int)parameters[0], (int)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToKama((int)parameters[0], (int)parameters[1], (int)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Kama).GetMethod("ToKama", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToKama should exist");
            directResults = (IReadOnlyList<KamaResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
