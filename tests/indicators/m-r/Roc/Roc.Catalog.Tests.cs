using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Roc catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class RocTests : TestBase
{
    [TestMethod]
    public void RocSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Roc.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<RocResult> catalogResults = listing.Execute<RocResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<RocResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToRoc();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToRoc(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToRoc(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToRoc(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Roc).GetMethod("ToRoc", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToRoc should exist");
            directResults = (IReadOnlyList<RocResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
