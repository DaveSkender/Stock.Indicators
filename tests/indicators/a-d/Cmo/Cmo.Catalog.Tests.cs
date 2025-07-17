using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Cmo catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class CmoTests : TestBase
{
    [TestMethod]
    public void CmoSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Cmo.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<CmoResult> catalogResults = listing.Execute<CmoResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<CmoResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToCmo();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToCmo((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToCmo(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToCmo(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Cmo).GetMethod("ToCmo", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToCmo should exist");
            directResults = (IReadOnlyList<CmoResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
