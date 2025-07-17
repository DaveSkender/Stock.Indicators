using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Chandelier catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ChandelierTests : TestBase
{
    [TestMethod]
    public void ChandelierSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Chandelier.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ChandelierResult> catalogResults = listing.Execute<ChandelierResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<ChandelierResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToChandelier();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToChandelier((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToChandelier((int)parameters[0], (double)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToChandelier((int)parameters[0], (double)parameters[1], (Direction)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Chandelier).GetMethod("ToChandelier", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToChandelier should exist");
            directResults = (IReadOnlyList<ChandelierResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
