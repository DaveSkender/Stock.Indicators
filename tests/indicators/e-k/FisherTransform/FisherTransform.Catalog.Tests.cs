using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for FisherTransform catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class FisherTransformTests : TestBase
{
    [TestMethod]
    public void FisherTransformSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = FisherTransform.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<FisherTransformResult> catalogResults = listing.Execute<FisherTransformResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<FisherTransformResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToFisherTransform();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToFisherTransform((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToFisherTransform(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToFisherTransform(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(FisherTransform).GetMethod("ToFisherTransform", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToFisherTransform should exist");
            directResults = (IReadOnlyList<FisherTransformResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
