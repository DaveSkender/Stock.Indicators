using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for StochRsi catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class StochRsiTests : TestBase
{
    [TestMethod]
    public void StochRsiSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = StochRsi.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<StochRsiResult> catalogResults = listing.Execute<StochRsiResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<StochRsiResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToStochRsi();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToStochRsi(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToStochRsi(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToStochRsi(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(StochRsi).GetMethod("ToStochRsi", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToStochRsi should exist");
            directResults = (IReadOnlyList<StochRsiResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
