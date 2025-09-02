namespace Catalogging;

/// <summary>
/// Catalog integrity tests enforcing the one-listing-per-style approach and related constraints:
/// - unique UIID+Style across the catalog
/// - multi-style indicators have separate listings with consistent base metadata
/// - single-style indicators appear once
/// - style distribution totals reconcile
/// </summary>
[TestClass]
public class CatalogIntegrityTests : TestBase
{
    [TestMethod]
    public void StaticCatalogShouldFollowOneListingPerStylePattern()
    {
        IReadOnlyCollection<IndicatorListing> allListings = Catalog.Get();
        var groupedByUiidAndStyle = allListings.GroupBy(listing => new { listing.Uiid, listing.Style }).ToList();
        foreach (var group in groupedByUiidAndStyle)
        {
            group.Should().HaveCount(1, $"UIID '{group.Key.Uiid}' with style '{group.Key.Style}' should appear exactly once in the catalog");
        }
    }

    [TestMethod]
    public void MultiStyleIndicatorsShouldHaveSeparateListings()
    {
        IReadOnlyCollection<IndicatorListing> emaListings = Catalog.Get("EMA");
        emaListings.Should().NotBeEmpty("EMA should exist in the catalog");
        List<IGrouping<Style, IndicatorListing>> styleGroups = emaListings.GroupBy(l => l.Style).ToList();
        styleGroups.Should().HaveCountGreaterThan(1, "EMA should have multiple styles");
        foreach (IGrouping<Style, IndicatorListing> styleGroup in styleGroups)
        {
            styleGroup.Should().HaveCount(1, $"EMA should have exactly one listing for style {styleGroup.Key}");
            styleGroup.All(l => l.Uiid == "EMA").Should().BeTrue();
            styleGroup.All(l => l.Name == "Exponential Moving Average").Should().BeTrue();
        }
    }

    [TestMethod]
    public void StyleSpecificListingsShouldHaveConsistentMetadata()
    {
        IReadOnlyCollection<IndicatorListing> smaListings = Catalog.Get("SMA");
        if (smaListings.Count > 1)
        {
            string baseName = smaListings.First().Name;
            Category baseCategory = smaListings.First().Category;
            foreach (IndicatorListing listing in smaListings)
            {
                listing.Name.Should().Be(baseName);
                listing.Category.Should().Be(baseCategory);
                listing.Uiid.Should().Be("SMA");
            }
        }
    }

    [TestMethod]
    public void SingleStyleIndicatorsShouldHaveOneListing()
    {
        IReadOnlyCollection<IndicatorListing> rsiListings = Catalog.Get("RSI");
        rsiListings.Should().NotBeEmpty();
        rsiListings.Should().HaveCount(1);
        rsiListings.First().Style.Should().Be(Style.Series);
    }

    [TestMethod]
    public void CatalogShouldHaveBalancedStyleDistribution()
    {
        IReadOnlyCollection<IndicatorListing> allListings = Catalog.Get();
        IReadOnlyCollection<IndicatorListing> seriesListings = Catalog.Get(Style.Series);
        IReadOnlyCollection<IndicatorListing> streamListings = Catalog.Get(Style.Stream);
        IReadOnlyCollection<IndicatorListing> bufferListings = Catalog.Get(Style.Buffer);

        seriesListings.Should().NotBeEmpty();
        streamListings.Should().NotBeEmpty();
        bufferListings.Should().NotBeEmpty();

        seriesListings.Count.Should().BeGreaterThan(streamListings.Count);
        seriesListings.Count.Should().BeGreaterThan(bufferListings.Count);
        (seriesListings.Count + streamListings.Count + bufferListings.Count).Should().Be(allListings.Count);
    }
}
