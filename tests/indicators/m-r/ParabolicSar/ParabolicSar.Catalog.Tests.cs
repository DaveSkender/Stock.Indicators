using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for ParabolicSar catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ParabolicSarTests : TestBase
{
    [TestMethod]
    public void ParabolicSarSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = ParabolicSar.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ParabolicSarResult> catalogResults = listing.Execute<ParabolicSarResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<ParabolicSarResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToParabolicSar();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToParabolicSar((double)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToParabolicSar((double)parameters[0], (double)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToParabolicSar((double)parameters[0], (double)parameters[1], (double)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(ParabolicSar).GetMethod("ToParabolicSar", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToParabolicSar should exist");
            directResults = (IReadOnlyList<ParabolicSarResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
