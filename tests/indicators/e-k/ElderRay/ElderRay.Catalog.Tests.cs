using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for ElderRay catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ElderRayTests : TestBase
{
    [TestMethod]
    public void ElderRaySeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = ElderRay.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ElderRayResult> catalogResults = listing.Execute<ElderRayResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<ElderRayResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToElderRay();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToElderRay((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToElderRay(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToElderRay(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(ElderRay).GetMethod("ToElderRay", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToElderRay should exist");
            directResults = (IReadOnlyList<ElderRayResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
