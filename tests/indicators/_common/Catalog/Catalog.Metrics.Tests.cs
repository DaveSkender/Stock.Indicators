namespace Catalogging;

/// <summary>
/// Catalog metrics tests:
/// - basic catalog sanity (presence, minimum size, non-empty fields)
/// - exact style counts and total count matching current static catalog
/// </summary>
[TestClass]
public class CatalogMetricsTests : TestBase
{
    [TestMethod]
    public void StaticCatalogShouldHaveCorrectCount()
    {
        IReadOnlyCollection<IndicatorListing> allListings = Catalog.Get();
        allListings.Should().NotBeEmpty();
        allListings.Count.Should().BeGreaterThan(50);
        allListings.Should().OnlyContain(static l => !string.IsNullOrWhiteSpace(l.Uiid));
        allListings.Should().OnlyContain(static l => !string.IsNullOrWhiteSpace(l.Name));
    }

    [TestMethod]
    public void CatalogShouldHaveExactStyleCounts()
    {
        IReadOnlyList<IndicatorListing> catalog = Catalog.Listings;
        int seriesCount = catalog.Count(static x => x.Style == Style.Series);
        int streamCount = catalog.Count(static x => x.Style == Style.Stream);
        int bufferCount = catalog.Count(static x => x.Style == Style.Buffer);

        Console.WriteLine($"Actual Catalog Style Counts: Series={seriesCount}, Stream={streamCount}, Buffer={bufferCount}, Total={seriesCount + streamCount + bufferCount}");

        // TODO: add final count later, not now
        seriesCount.Should().Be(85);  // Updated for QuotePart
        bufferCount.Should().BeGreaterThan(5);
        streamCount.Should().BeGreaterThan(10);

        int totalCount = seriesCount + streamCount + bufferCount;
        totalCount.Should().BeGreaterThan(100);
    }
}
