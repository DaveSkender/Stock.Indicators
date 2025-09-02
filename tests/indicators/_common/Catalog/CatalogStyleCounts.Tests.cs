namespace Catalog;

[TestClass]
public class CatalogStyleCounts : TestBase
{
    [TestMethod]
    public void CatalogShouldHaveCorrectStyleCounts()
    {
        // Arrange & Act
        IReadOnlyList<IndicatorListing> catalog = IndicatorCatalog.Catalog;

        int seriesCount = catalog.Count(x => x.Style == Style.Series);
        int streamCount = catalog.Count(x => x.Style == Style.Stream);
        int bufferCount = catalog.Count(x => x.Style == Style.Buffer);

        // Output actual counts for debugging
        Console.WriteLine($"Actual Catalog Style Counts: Series={seriesCount}, Stream={streamCount}, Buffer={bufferCount}, Total={seriesCount + streamCount + bufferCount}");

        // Assert - use exact counts based on current catalog
        seriesCount.Should().Be(84, "there should be exactly 84 series style indicators");
        bufferCount.Should().Be(2, "there should be exactly 2 buffer style indicators");
        streamCount.Should().Be(8, "there should be exactly 8 stream style indicators");

        // Total verification
        int totalCount = seriesCount + streamCount + bufferCount;
        totalCount.Should().Be(94, "total indicators should be exactly 94");
    }
}
