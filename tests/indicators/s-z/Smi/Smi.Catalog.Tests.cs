using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Smi catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class SmiTests : TestBase
{
    [TestMethod]
    public void SmiSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Smi.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<SmiResult> catalogResults = listing.Execute<SmiResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<SmiResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToSmi();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToSmi(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToSmi(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToSmi(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Smi).GetMethod("ToSmi", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToSmi should exist");
            directResults = (IReadOnlyList<SmiResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
