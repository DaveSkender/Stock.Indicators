using System.Reflection;


namespace Catalog;

/// <summary>
/// Test class for Alma catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class AlmaTests : TestBase
{
    [TestMethod]
    public void AlmaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Alma.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<AlmaResult> catalogResults = listing.Execute<AlmaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<AlmaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToAlma();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToAlma(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToAlma(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToAlma(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Alma).GetMethod("ToAlma", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToAlma should exist");
            directResults = (IReadOnlyList<AlmaResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
