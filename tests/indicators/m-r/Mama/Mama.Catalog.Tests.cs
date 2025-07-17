using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Mama catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class MamaTests : TestBase
{
    [TestMethod]
    public void MamaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Mama.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<MamaResult> catalogResults = listing.Execute<MamaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<MamaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToMama();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToMama((double)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToMama((double)parameters[0], (double)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToMama(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Mama).GetMethod("ToMama", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToMama should exist");
            directResults = (IReadOnlyList<MamaResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
