using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Aroon catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class AroonTests : TestBase
{
    [TestMethod]
    public void AroonSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Aroon.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<AroonResult> catalogResults = listing.Execute<AroonResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<AroonResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToAroon();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToAroon((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToAroon(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToAroon(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Aroon).GetMethod("ToAroon", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToAroon should exist");
            directResults = (IReadOnlyList<AroonResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
