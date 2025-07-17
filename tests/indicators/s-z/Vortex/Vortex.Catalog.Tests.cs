using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Vortex catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class VortexTests : TestBase
{
    [TestMethod]
    public void VortexSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Vortex.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<VortexResult> catalogResults = listing.Execute<VortexResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<VortexResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToVortex();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToVortex((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToVortex(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToVortex(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Vortex).GetMethod("ToVortex", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToVortex should exist");
            directResults = (IReadOnlyList<VortexResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
