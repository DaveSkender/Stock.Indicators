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
    public void CatalogHasCategoryContent()
    {
        // Act - get catalog
        IReadOnlyList<IndicatorListing> result = Catalog.Get();

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
    public void CatalogHasRealIndicators()
    {
        // Act - get catalog
        IReadOnlyList<IndicatorListing> realCatalog = Catalog.Get();

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

        realCatalog.Should().NotContain(x =>
            x.Uiid.Contains("_TEST"),
            "Catalog should not contain test indicators");

        realCatalog.Count.Should()
            .BeGreaterThan(80, "The catalog should contain all the indicators");

        // Verify UIIDs are unique (replacing the previous Catalog.Validate call)
        realCatalog.GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .Should().BeEmpty("Catalog should have unique UIIDs");
    }

    [TestMethod]
    public void CatalogIncludesBaseUrl()
    {
        // Act - get catalog with URL endpoints
        IReadOnlyList<IndicatorListing> result = Catalog.Get(BaseUrl);

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
    public void DuplicateUiidDetection()
    {
        // Act - Get the standard catalog
        IReadOnlyList<IndicatorListing> standardCatalog = Catalog.Get();

        // Create a custom catalog with a duplicate UIID
        List<IndicatorListing> customCatalog = [.. standardCatalog];

        // Find any existing indicator to duplicate
        IndicatorListing indicatorToDuplicate = standardCatalog[0];

        // Create a duplicate with the same UIID but different name
        IndicatorListing duplicateIndicator = indicatorToDuplicate
            with { Name = "Duplicate of " + indicatorToDuplicate.Name };

        // Add the duplicate to the custom catalog
        customCatalog.Add(duplicateIndicator);

        // Find duplicate UIIDs in the customCatalog
        List<string> duplicateUiids = customCatalog
            .GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        // Assert
        duplicateUiids.Should().NotBeEmpty("Should detect duplicate UIIDs in modified catalog");
        duplicateUiids.Should().Contain(indicatorToDuplicate.Uiid,
            $"Should identify {indicatorToDuplicate.Uiid} as a duplicate UIID");

        // Important: Verify that the standard catalog does NOT have duplicate UIIDs
        // This confirms that the Catalog.Get() method returns a catalog with unique UIIDs
        standardCatalog.GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .Should().BeEmpty("Standard catalog should not have duplicate UIIDs");
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
        IReadOnlyList<IndicatorListing> catalog = Catalog.Get();

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

        json.Should().NotContain("_TEST", "Should not contain test indicators");
        catalog.Count.Should().BeGreaterThan(
            80, "The catalog should contain all the indicators");

        Console.WriteLine($"Catalog with {catalog.Count} indicators exported to {jsonPath}");
    }
}
