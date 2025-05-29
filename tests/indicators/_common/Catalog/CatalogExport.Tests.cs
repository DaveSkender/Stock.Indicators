using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Skender.Stock.Indicators;

namespace Catalog;

[TestClass]
public class CatalogExport : TestBase
{
    private readonly JsonSerializerOptions IndentedJsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // the generated catalog.json test exported file (below)
    private readonly string jsonPath = Path.Combine(Path.GetFullPath(Path.Combine(
        Directory.GetCurrentDirectory(), "..", "..", "..", "_common", "Catalog")), "catalog.json");

    [TestMethod]
    public void ExportCatalogToJsonFile()
    {
        // Skip in CI/CD pipeline, only for local debugging
        if (Environment.GetEnvironmentVariable("CI") == "true")
        {
            return;
        }

        // Arrange: Clear existing registrations and force full catalog registration
        IndicatorRegistry.Clear();

        // Force registration from both catalog listings and attributes
        IndicatorRegistry.RegisterCatalog();
        IndicatorRegistry.RegisterAuto();

        // Act: get standard catalog by calling the internal method that bypasses test detection
        IReadOnlyCollection<IndicatorListing> catalog = IndicatorRegistry.GetAllIndicators();

        // Serialize to JSON for inspection
        string json = JsonSerializer.Serialize(catalog, IndentedJsonOptions);

        // Write to file
        File.WriteAllText(jsonPath, json);

        // Assert that the catalog contains real indicators
        catalog.Should().NotBeEmpty("The catalog should have content");

        json.Should().NotContain("_TEST", "Should not contain test indicators");
        catalog.Count.Should().BeGreaterThan(
            80, "The catalog should contain all the indicators");

        Console.WriteLine($"Catalog with {catalog.Count} indicators exported to {jsonPath}");

        // Assert that the file re-imports and serializes correctly
        IReadOnlyList<IndicatorListing> imported
            = JsonSerializer.Deserialize<IReadOnlyList<IndicatorListing>>(File.ReadAllText(jsonPath));

        imported.Should().BeEquivalentTo(catalog,
            "The imported catalog should match the original catalog");
    }
}
