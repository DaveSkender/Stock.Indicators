using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Atr catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class AtrTests : TestBase
{
    [TestMethod]
    public void AtrSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Atr.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<AtrResult> catalogResults = listing.Execute<AtrResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<AtrResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToAtr();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToAtr(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToAtr(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToAtr(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Atr).GetMethod("ToAtr", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToAtr should exist");
            directResults = (IReadOnlyList<AtrResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
