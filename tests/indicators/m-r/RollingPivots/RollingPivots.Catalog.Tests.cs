using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for RollingPivots catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class RollingPivotsTests : TestBase
{
    [TestMethod]
    public void RollingPivotsSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = RollingPivots.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<RollingPivotsResult> catalogResults = listing.Execute<RollingPivotsResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<RollingPivotsResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToRollingPivots();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToRollingPivots(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToRollingPivots((int)parameters[0], (int)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToRollingPivots((int)parameters[0], (int)parameters[1], (PivotPointType)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(RollingPivots).GetMethod("ToRollingPivots", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToRollingPivots should exist");
            directResults = (IReadOnlyList<RollingPivotsResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
