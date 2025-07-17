using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Tsi catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class TsiTests : TestBase
{
    [TestMethod]
    public void TsiSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Tsi.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<TsiResult> catalogResults = listing.Execute<TsiResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<TsiResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToTsi();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToTsi((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToTsi((int)parameters[0], (int)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToTsi((int)parameters[0], (int)parameters[1], (int)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Tsi).GetMethod("ToTsi", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToTsi should exist");
            directResults = (IReadOnlyList<TsiResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
