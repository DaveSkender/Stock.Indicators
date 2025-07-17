using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for BollingerBands catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class BollingerBandsTests : TestBase
{
    [TestMethod]
    public void BollingerBandsSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = BollingerBands.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<BollingerBandsResult> catalogResults = listing.Execute<BollingerBandsResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<BollingerBandsResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToBollingerBands();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToBollingerBands((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToBollingerBands((int)parameters[0], (double)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToBollingerBands(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(BollingerBands).GetMethod("ToBollingerBands", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToBollingerBands should exist");
            directResults = (IReadOnlyList<BollingerBandsResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
