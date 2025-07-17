using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for StdDevChannels catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class StdDevChannelsTests : TestBase
{
    [TestMethod]
    public void StdDevChannelsSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = StdDevChannels.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<StdDevChannelsResult> catalogResults = listing.Execute<StdDevChannelsResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<StdDevChannelsResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToStdDevChannels();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToStdDevChannels((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToStdDevChannels((int)parameters[0], (double)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToStdDevChannels(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(StdDevChannels).GetMethod("ToStdDevChannels", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToStdDevChannels should exist");
            directResults = (IReadOnlyList<StdDevChannelsResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
