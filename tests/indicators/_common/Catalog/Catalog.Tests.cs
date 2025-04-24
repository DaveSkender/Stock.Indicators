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
    public void TestCatalogHasStubs()
    {
        // Act - get catalog with test indicators included
        IReadOnlyList<IndicatorListing> result = Catalog.GetIndicators(includeTestIndicators: true);

        // Assert
        // Check that the catalog contains indicators from different categories
        result.Should().NotBeEmpty("Catalog should contain indicators");

        // Verify that both real indicators and test indicators are present
        result.Should().Contain(x => x.Category == Category.MovingAverage,
            "Catalog should include real MovingAverage indicators");
        result.Should().Contain(x => x.Category == Category.Oscillator,
            "Catalog should include real Oscillator indicators");
        result.Should().Contain(x => x.Category == Category.PriceChannel,
            "Catalog should include real PriceChannel indicators");
        result.Should().Contain(x => x.Category == Category.VolumeBased,
            "Catalog should include real VolumeBased indicators");

        // Test indicators should be included when the includeTestIndicators parameter is true
        result.Should().Contain(x => x.Uiid == "GEN_TEST",
            "Catalog with test indicators should include GEN_TEST");
        result.Should().Contain(x => x.Uiid == "BUFFER_TEST",
            "Catalog with test indicators should include BUFFER_TEST");
        result.Should().Contain(x => x.Uiid == "STREAM_TEST",
            "Catalog with test indicators should include STREAM_TEST");
        result.Should().Contain(x => x.Uiid == "SERIES_TEST",
            "Catalog with test indicators should include SERIES_TEST");
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

        // Test indicators should never be part of the public catalog
        result.Should().NotContain(x => x.Uiid == "GEN_TEST",
            "Public catalog should not contain the GEN_TEST indicator");
        result.Should().NotContain(x => x.Uiid == "BUFFER_TEST",
            "Public catalog should not contain the BUFFER_TEST indicator");
        result.Should().NotContain(x => x.Uiid == "STREAM_TEST",
            "Public catalog should not contain the STREAM_TEST indicator");

        // Ensure catalog is valid
        Catalog.ValidateUniqueUIID(result);
    }

    [TestMethod]
    public void CatalogWithRealIndicators()
    {
        // Act - get catalog without test indicators
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

        // Ensure UIID values are unique
        Catalog.ValidateUniqueUIID(realCatalog);

        // Check that test indicators are not included
        realCatalog.Should().NotContain(GeneratedCatalog.TestIndicators,
            "Catalog should not have test indicators.");
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

        // Act and Assert
        Action act = () => Catalog.ValidateUniqueUIID(catalog);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Duplicate UIIDs found*")
            .And.Message.Should().Contain("TEST");
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

        // Test indicators are excluded
        catalog.Should().NotContain(x => x.Uiid == "GEN_TEST", "Public API should exclude GEN_TEST");
        catalog.Should().NotContain(x => x.Uiid == "BUFFER_TEST", "Public API should exclude BUFFER_TEST");
        catalog.Should().NotContain(x => x.Uiid == "STREAM_TEST", "Public API should exclude STREAM_TEST");
        catalog.Should().NotContain(x => x.Uiid == "SERIES_TEST", "Public API should exclude SERIES_TEST");

        json.Should().NotContain("BUFFER_TEST", "Should not contain the 'stub' catalog");
        catalog.Count.Should().BeGreaterThan(
            80, "The catalog should contain all the indicators");

        Console.WriteLine($"Catalog with {catalog.Count} indicators exported to {jsonPath}");
    }
}
