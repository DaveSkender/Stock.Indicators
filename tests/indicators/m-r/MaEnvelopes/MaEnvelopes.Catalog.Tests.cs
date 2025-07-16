using System.Reflection;

namespace Catalog;

/// <summary>
/// Test class for MaEnvelopes catalog functionality.
/// Validates that catalog-based execution produces identical results to direct calls.
/// </summary>
[TestClass]
public class MaEnvelopesTests : TestBase
{
    [TestMethod]
    public void MaEnvelopesSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes;
        IndicatorListing listing = MaEnvelopes.SeriesListing;
        listing.Should().NotBeNull();

        // Act - Call using catalog metadata
        IReadOnlyList<MaEnvelopesResult> catalogResults = listing.Execute<MaEnvelopesResult>(quotes);

        // Act - Direct call for comparison using default parameters
        var parameters = listing.Parameters?.Where(p => p.IsRequired && p.DefaultValue != null)
            .Select(p => p.DefaultValue).ToArray() ?? new object[0];
        
        IReadOnlyList<MaEnvelopesResult> directResults;
        if (parameters.Length == 0)
        {
            directResults = quotes.ToMaEnvelopes();
        }
        else if (parameters.Length == 1)
        {
            directResults = quotes.ToMaEnvelopes(parameters[0]);
        }
        else if (parameters.Length == 2)
        {
            directResults = quotes.ToMaEnvelopes(parameters[0], parameters[1]);
        }
        else if (parameters.Length == 3)
        {
            directResults = quotes.ToMaEnvelopes(parameters[0], parameters[1], parameters[2]);
        }
        else
        {
            // Use reflection for complex parameter cases
            var method = typeof(MaEnvelopes).GetMethod("ToMaEnvelopes", BindingFlags.Public | BindingFlags.Static);
            method.Should().NotBeNull("Method ToMaEnvelopes should exist");
            directResults = (IReadOnlyList<MaEnvelopesResult>)method!.Invoke(null, 
                new object[] { quotes }.Concat(parameters).ToArray());
        }

        // Assert - Results should be identical
        catalogResults.Should().BeEquivalentTo(directResults);
    }
}
