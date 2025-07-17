using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for ChaikinOsc catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ChaikinOscTests : TestBase
{
    [TestMethod]
    public void ChaikinOscSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = ChaikinOsc.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ChaikinOscResult> catalogResults = listing.Execute<ChaikinOscResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<ChaikinOscResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToChaikinOsc();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToChaikinOsc((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToChaikinOsc((int)parameters[0], (int)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToChaikinOsc(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(ChaikinOsc).GetMethod("ToChaikinOsc", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToChaikinOsc should exist");
            directResults = (IReadOnlyList<ChaikinOscResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
