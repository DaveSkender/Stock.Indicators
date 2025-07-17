using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Pivots catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class PivotsTests : TestBase
{
    [TestMethod]
    public void PivotsSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Pivots.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<PivotsResult> catalogResults = listing.Execute<PivotsResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<PivotsResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToPivots();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToPivots((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToPivots((int)parameters[0], (int)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToPivots((int)parameters[0], (int)parameters[1], (int)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Pivots).GetMethod("ToPivots", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToPivots should exist");
            directResults = (IReadOnlyList<PivotsResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
