using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Cmf catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class CmfTests : TestBase
{
    [TestMethod]
    public void CmfSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Cmf.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<CmfResult> catalogResults = listing.Execute<CmfResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<CmfResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToCmf();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToCmf((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToCmf(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToCmf(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Cmf).GetMethod("ToCmf", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToCmf should exist");
            directResults = (IReadOnlyList<CmfResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
