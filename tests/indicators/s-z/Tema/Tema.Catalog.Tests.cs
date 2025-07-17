using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Tema catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class TemaTests : TestBase
{
    [TestMethod]
    public void TemaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Tema.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<TemaResult> catalogResults = listing.Execute<TemaResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<TemaResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToTema();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToTema((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToTema(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToTema(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Tema).GetMethod("ToTema", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToTema should exist");
            directResults = (IReadOnlyList<TemaResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
