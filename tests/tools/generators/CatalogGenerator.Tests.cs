namespace GeneratedCatalog;

[TestClass]
public class CatalogGenerating
{
    [TestMethod]
    public void CatalogGenerator_IncludesIndicatorsWithAttributes()
    {
        // Arrange/Act - access the generated catalog
        IReadOnlyList<IndicatorListing> catalog = Catalog.Get();

        // Assert
        catalog.Should().NotBeEmpty("Catalog should not be empty");
        catalog.Should().Contain(x => x.Name.Contains("Simple Moving Average") && x.Uiid == "SMA",
            "SMA indicator should be in the catalog");
    }

    [TestMethod]
    public void CatalogGenerator_UsesLegendOverrideFromAttribute()
    {
        // Arrange/Act - access the generated catalog
        IReadOnlyList<IndicatorListing> catalog = Catalog.Get();

        // Check a regular indicator with parameters
        IndicatorListing? smaIndicator = catalog.FirstOrDefault(x => x.Uiid == "SMA");
        smaIndicator.Should().NotBeNull("SMA indicator should exist");

        if (smaIndicator != null && smaIndicator.Parameters?.Any() == true)
        {
            // The legendTemplate for SMA should follow the default format with parameters
            smaIndicator.LegendTemplate.Should().StartWith("SMA(",
                "LegendTemplate should use the default format with parameters");
            smaIndicator.LegendTemplate.Should().Contain("[P",
                "LegendTemplate should include parameter placeholders");
        }
    }
}
