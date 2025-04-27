using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utilities;

[TestClass]
public class Catalogging
{
    private static readonly Uri BaseUrl = new("https://example.com");

    private readonly JsonSerializerOptions IndentedJsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [TestMethod]
    public void CatalogHasRealIndicators()
    {
        // Act - get catalog
        IReadOnlyList<IndicatorListing> result = Catalog.GetIndicators();

        // Assert
        result.Should().NotBeEmpty("Catalog should contain indicators");

        // Verify that indicators from different categories are present
        result.Should().Contain(x => x.Category == Category.MovingAverage,
            "Catalog should include MovingAverage indicators");
        result.Should().Contain(x => x.Category == Category.Oscillator,
            "Catalog should include Oscillator indicators");
        result.Should().Contain(x => x.Category == Category.PriceChannel,
            "Catalog should include PriceChannel indicators");
        result.Should().Contain(x => x.Category == Category.VolumeBased,
            "Catalog should include VolumeBased indicators");
    }

    [TestMethod]
    public void CatalogWithBaseUrl()
    {
        // Act - get catalog with URL endpoints
        IReadOnlyList<IndicatorListing> result = Catalog.GetIndicatorsWithEndpoints(BaseUrl);

        // Assert
        // Verify all indicators have the base URL applied
        foreach (IndicatorListing indicator in result)
        {
            indicator.UiidEndpoint.Should().StartWith(BaseUrl.ToString(),
                $"All endpoints should start with base URL: {indicator.UiidEndpoint}");
        }

        // General assertions
        result.Should().NotBeEmpty("Catalog should not be empty");

        // Check for various categories
        result.Should().Contain(x => x.Category == Category.MovingAverage,
            "Catalog should include MovingAverage indicators");
        result.Should().Contain(x => x.Category == Category.Oscillator,
            "Catalog should include Oscillator indicators");
        result.Should().Contain(x => x.Category == Category.PriceChannel,
            "Catalog should include PriceChannel indicators");
        result.Should().Contain(x => x.Category == Category.VolumeBased,
            "Catalog should include VolumeBased indicators");

        // Verify UIIDs are unique (replacing the previous Catalog.Validate call)
        result.GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .Should().BeEmpty("Catalog should have unique UIIDs");
    }

    [TestMethod]
    public void CatalogWithRealIndicators()
    {
        // Act - get catalog
        IReadOnlyList<IndicatorListing> realCatalog = Catalog.GetIndicators();

        // Assert
        realCatalog.Should().NotBeEmpty("Catalog should contain indicators");

        // Check for specific indicators we expect
        realCatalog.Should().Contain(x =>
            x.Uiid == "SMA" &&
            x.Name.Contains("Simple Moving Average") &&
            x.Category == Category.MovingAverage,
            "Catalog should include SMA indicator");

        realCatalog.Should().Contain(x =>
            x.Uiid == "RSI" &&
            x.Name.Contains("Relative Strength") &&
            x.Category == Category.Oscillator,
            "Catalog should include RSI indicator");

        realCatalog.Should().Contain(x =>
            x.Uiid == "BB" &&
            x.Name.Contains("Bollinger") &&
            x.Category == Category.PriceChannel,
            "Catalog should include Bollinger Bands");

        // Verify UIIDs are unique (replacing the previous Catalog.Validate call)
        realCatalog.GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .Should().BeEmpty("Catalog should have unique UIIDs");
    }

    [TestMethod]
    public void DuplicateUiidDetection()
    {
        // Arrange
        IndicatorListing duplicateIndicator = new() {
            Name = "Duplicate Indicator",
            Uiid = "DUPLICATE",
            Category = Category.PriceCharacteristic,
            ChartType = ChartType.Overlay,
            LegendTemplate = "DUPLICATE([P1])",
            Parameters = [],
            Results = [
                new IndicatorResultConfig
                {
                    DisplayName = "Test Result",
                    TooltipTemplate = "TEST([P1])",
                    DataName = "testResult",
                    DataType = "number",
                    LineType = "solid",
                    DefaultColor = ChartColors.StandardBlue
                }
            ]
        };

        // Adding the same indicator twice to simulate duplicate UIID
        List<IndicatorListing> catalog = [
            duplicateIndicator,
            duplicateIndicator
        ];

        // Act - find any duplicate UIIDs
        var duplicateUiids = catalog
            .GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Assert
        duplicateUiids.Should().NotBeEmpty("Should detect duplicate UIIDs");
        duplicateUiids.Should().Contain("DUPLICATE", "Should identify the specific duplicate UIID");
    }

    [TestMethod]
    public void ExportCatalogToJsonFile()
    {
        // Skip in CI/CD pipeline, only for local debugging
        if (Environment.GetEnvironmentVariable("CI") == "true")
        {
            return;
        }

        // Act - get standard catalog
        IReadOnlyList<IndicatorListing> catalog = Catalog.GetIndicators();

        // Serialize to JSON for inspection
        string json = JsonSerializer.Serialize(catalog, IndentedJsonOptions);

        // Get the source folder path (not the build output folder)
        // Using GetCurrentDirectory to get the root directory of the project
        string sourceFolder = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(), "..", "..", "..", "_common", "Catalog"));

        // Ensure the directory exists
        Directory.CreateDirectory(sourceFolder);

        // Save to file in the same directory as the test source code
        string jsonPath = Path.Combine(sourceFolder, "catalog.json");

        // Write to file
        File.WriteAllText(jsonPath, json);

        // Assert that the catalog contains real indicators
        catalog.Should().NotBeEmpty("The catalog should have content");

        json.Should().NotContain("BUFFER_TEST", "Should not contain test indicators");
        catalog.Count.Should().BeGreaterThan(
            80, "The catalog should contain all the indicators");

        Console.WriteLine($"Catalog with {catalog.Count} indicators exported to {jsonPath}");
    }
}
