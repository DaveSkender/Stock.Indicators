using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for RenkoAtr catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class RenkoAtrTests : TestBase
{
    [TestMethod]
    public void RenkoAtrSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = RenkoAtr.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<RenkoAtrResult> catalogResults = listing.Execute<RenkoAtrResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<RenkoAtrResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToRenkoAtr();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToRenkoAtr(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToRenkoAtr(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToRenkoAtr(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(RenkoAtr).GetMethod("ToRenkoAtr", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToRenkoAtr should exist");
            directResults = (IReadOnlyList<RenkoAtrResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}