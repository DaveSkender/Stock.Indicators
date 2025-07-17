using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Chop catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ChopTests : TestBase
{
    [TestMethod]
    public void ChopSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Chop.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ChopResult> catalogResults = listing.Execute<ChopResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<ChopResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToChop();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToChop((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToChop(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToChop(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Chop).GetMethod("ToChop", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToChop should exist");
            directResults = (IReadOnlyList<ChopResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
