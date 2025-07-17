using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for ConnorsRsi catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class ConnorsRsiTests : TestBase
{
    [TestMethod]
    public void ConnorsRsiSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = ConnorsRsi.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<ConnorsRsiResult> catalogResults = listing.Execute<ConnorsRsiResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? [];

        IReadOnlyList<ConnorsRsiResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToConnorsRsi();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToConnorsRsi((int)parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToConnorsRsi((int)parameters[0], (int)parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToConnorsRsi((int)parameters[0], (int)parameters[1], (int)parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(ConnorsRsi).GetMethod("ToConnorsRsi", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToConnorsRsi should exist");
            directResults = (IReadOnlyList<ConnorsRsiResult>)method!.Invoke(null,
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
