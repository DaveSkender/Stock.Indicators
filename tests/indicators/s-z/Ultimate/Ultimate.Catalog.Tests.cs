using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Ultimate catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class UltimateTests : TestBase
{
    [TestMethod]
    public void UltimateSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Ultimate.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<UltimateResult> catalogResults = listing.Execute<UltimateResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<UltimateResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToUltimate();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToUltimate((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToUltimate((int)parameters[0], (int)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToUltimate((int)parameters[0], (int)parameters[1], (int)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Ultimate).GetMethod("ToUltimate", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToUltimate should exist");
            directResults = (IReadOnlyList<UltimateResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
