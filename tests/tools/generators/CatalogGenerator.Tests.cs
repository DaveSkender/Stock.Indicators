namespace GeneratedCatalog;

[TestClass]
public class CatalogGenerating
{
    [TestMethod]
    public void CatalogGenerator_MethodWithSeriesAttribute_IsIdentified()
    {
        // Arrange/Act - access the generated catalog
        IReadOnlyList<IndicatorListing> catalog = Catalog.GetIndicators(includeTestIndicators: true);

        // Assert
        catalog.Should().NotBeEmpty("Catalog should not be empty");
        catalog.Should().Contain(x => x.Name.Contains("Simple Moving Average") && x.Uiid == "SMA",
            "SMA indicator should be in the catalog");

        // Check for a test indicator
        catalog.Should().Contain(x => x.Uiid == "GEN_TEST",
            "Test indicators should be included when requested");
    }

    [TestMethod]
    public void CatalogGenerator_ExcludesTestIndicators_WhenSpecified()
    {
        // Arrange/Act - access the generated catalog without test indicators
        IReadOnlyList<IndicatorListing> catalog = Catalog.GetIndicators(includeTestIndicators: false);

        // Assert
        catalog.Should().NotBeEmpty("Catalog should not be empty");

        // Check that real indicators are included
        catalog.Should().Contain(x => x.Name.Contains("Simple Moving Average") && x.Uiid == "SMA",
            "SMA indicator should be in the catalog");

        // Check that test indicators are excluded
        catalog.Should().NotContain(x => x.Uiid == "GEN_TEST",
            "Test indicators should be excluded");
        catalog.Should().NotContain(x => x.Uiid == "BUFFER_TEST",
            "Test indicators should be excluded");
        catalog.Should().NotContain(x => x.Uiid == "STREAM_TEST",
            "Test indicators should be excluded");
        catalog.Should().NotContain(x => x.Uiid == "SERIES_TEST",
            "Test indicators should be excluded");
    }
}
