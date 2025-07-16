using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Epma catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class EpmaTests : TestBase
{
    [TestMethod]
    public void EpmaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Epma.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<EpmaResult> catalogResults = listing.Execute<EpmaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<EpmaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToEpma();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToEpma(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToEpma(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToEpma(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Epma).GetMethod("ToEpma", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToEpma should exist");
            directResults = (IReadOnlyList<EpmaResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
