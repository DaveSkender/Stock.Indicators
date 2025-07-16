using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for RocWb catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class RocWbTests : TestBase
{
    [TestMethod]
    public void RocWbSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = RocWb.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<RocWbResult> catalogResults = listing.Execute<RocWbResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<RocWbResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToRocWb();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToRocWb(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToRocWb(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToRocWb(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(RocWb).GetMethod("ToRocWb", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToRocWb should exist");
            directResults = (IReadOnlyList<RocWbResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
