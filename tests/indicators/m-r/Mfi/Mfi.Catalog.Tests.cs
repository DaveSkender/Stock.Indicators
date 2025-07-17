using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for Mfi catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class MfiTests : TestBase
{
    [TestMethod]
    public void MfiSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = Mfi.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<MfiResult> catalogResults = listing.Execute<MfiResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<MfiResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToMfi();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToMfi((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToMfi(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToMfi(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(Mfi).GetMethod("ToMfi", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToMfi should exist");
            directResults = (IReadOnlyList<MfiResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
