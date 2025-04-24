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

    [TestMethod]
    public void CatalogGenerator_UsesLegendOverrideFromAttribute()
    {
        // Arrange/Act - access the generated catalog with test indicators
        IReadOnlyList<IndicatorListing> catalog = Catalog.GetIndicators(includeTestIndicators: true);

        // Assert - find indicators that should have legendOverride values
        // Check a test indicator with legendOverride
        var testIndicatorWithCustomLegend = catalog.FirstOrDefault(x => x.Uiid == "GEN_TEST");
        testIndicatorWithCustomLegend.Should().NotBeNull("Test indicator should exist");

        // The legendTemplate should reflect the overridden value from the attribute
        if (testIndicatorWithCustomLegend != null)
        {
            testIndicatorWithCustomLegend.LegendTemplate.Should().NotBe("GEN_TEST",
                "LegendTemplate should not be the default ID when override exists");

            // Depending on what override value is set in test indicators
            testIndicatorWithCustomLegend.LegendTemplate.Should().Contain("TEST",
                "LegendTemplate should contain the overridden format from the attribute");
        }

        // Check a regular indicator without legendOverride (with parameters)
        var smaIndicator = catalog.FirstOrDefault(x => x.Uiid == "SMA");
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
