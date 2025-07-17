using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for PivotPoints catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class PivotPointsTests : TestBase
{
    [TestMethod]
    public void PivotPointsSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = PivotPoints.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<PivotPointsResult> catalogResults = listing.Execute<PivotPointsResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<PivotPointsResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToPivotPoints();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToPivotPoints((PeriodSize)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToPivotPoints((PeriodSize)parameters[0], (PivotPointType)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToPivotPoints(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(PivotPoints).GetMethod("ToPivotPoints", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToPivotPoints should exist");
            directResults = (IReadOnlyList<PivotPointsResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
