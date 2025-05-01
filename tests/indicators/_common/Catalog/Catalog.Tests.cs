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

    // the generated catalog.json test exported file (below)
    private readonly string jsonPath = Path.Combine(Path.GetFullPath(Path.Combine(
        Directory.GetCurrentDirectory(), "..", "..", "..", "_common", "Catalog")), "catalog.json");

    [TestMethod]
    public void CatalogHasCategoryContent()
    {
        // Act: get catalog
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
        // Act: get catalog
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
        // Act: get catalog with URL endpoints
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
        // Act: Get the standard catalog
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
    public void CatalogIncludesEnumValues()
    {
        // Arrange: define expected values for the EndType enum
        int minValue = (int)Enum.GetValues<EndType>().Min();
        int maxValue = (int)Enum.GetValues<EndType>().Max();
        Dictionary<int, string> enumValues
            = Enum.GetValues<EndType>()
             .ToDictionary(x => (int)x, x => x.ToString());

        minValue.Should().Be(0, "EndType enum should have a minimum value of 0");
        maxValue.Should().BeGreaterThan(0, "EndType enum should have a larger maximum value");
        enumValues.Should().NotBeEmpty("EndType enum should have values");
        enumValues.Should().ContainKey(minValue, $"EndType enum should contain a value for {minValue}");
        enumValues.Should().ContainKey(maxValue, $"EndType enum should contain a value for {maxValue}");
        enumValues.Should().ContainValue("Close", $"EndType enum should contain a value for {minValue}");
        enumValues.Should().HaveCountGreaterThan(1, "EndType enum should have more than one value");

        // Act: get catalog with URL endpoints
        IReadOnlyList<IndicatorListing> catalog = Catalog.Get();

        IndicatorListing listing = catalog.Single(x => x.Uiid == "ATR-STOP");

        IndicatorParamConfig param
            = listing.Parameters.Single(x => x.ParamName == "endType");

        // Assert: verify endType enum has correct min/max values
        param.DataType.Should().Be("enum", "EndType should be an 'enum'");

        param.Minimum.Should().Be(minValue, $"EndType enum should have a minimum value of {minValue}");
        param.Maximum.Should().Be(maxValue, $"EndType enum should have a maximum value of {maxValue}");

        param.EnumOptions.Should().BeEquivalentTo(enumValues,
            "EndType enum values should match the expected values");

        // Assert: verify that other indicators do not have enum values
        IndicatorListing macdListing = catalog.Single(x => x.Uiid == "MACD");

        for (int i = 0; i < macdListing.Parameters.Count; i++)
        {
            IndicatorParamConfig p = macdListing.Parameters[i];
            p.DataType.Should().NotBe("enum", "Non-enum parameters should not be enums");
            p.EnumOptions.Should().BeNull("Non-enum parameters should not have enum values");
        }
    }

    [TestMethod]
    public void CatalogParamsHaveCorrectPid()
    {
        // Act: get catalog with URL endpoints
        IReadOnlyList<IndicatorListing> catalog = Catalog.Get();
        IndicatorListing listing = catalog.Single(x => x.Uiid == "MACD");

        // Assert: verify expected PID, legend, and tooltip
        // (MACD has standard legend and tooltips patterns)

        for (int i = 0; i < listing.Parameters.Count; i++)
        {
            IndicatorParamConfig p = listing.Parameters[i];

            // correctly sequenced PIDs
            string pid = $"P{i + 1}";
            p.Pid.Should().Be(pid);

            // legend template should contain the PIDs
            listing.LegendTemplate.Should().Contain(pid,
                $"Legend template should contain '{pid}' for MACD");

            // tooltip template should contain the PIDs
            foreach (IndicatorResultConfig result in listing.Results)
            {
                result.TooltipTemplate.Should().Contain(pid,
                    $"Tooltip template should contain '{pid}' for MACD");
            }
        }
    }

    [TestMethod]
    public void ExportCatalogToJsonFile()
    {
        // Skip in CI/CD pipeline, only for local debugging
        if (Environment.GetEnvironmentVariable("CI") == "true")
        {
            return;
        }

        // Act: get standard catalog
        IReadOnlyList<IndicatorListing> catalog = Catalog.Get();

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

        IndicatorListing listing = catalog.Single(x => x.Uiid == "ATR-STOP");

        IndicatorParamConfig param
            = listing.Parameters.Single(x => x.ParamName == "endType");

        param.Minimum.Should().NotBeNull("EndType enum should have a minimum value");
        param.Maximum.Should().NotBeNull("EndType enum should have a maximum value");
        param.EnumOptions.Should().HaveCountGreaterThan(0, "EndType enum should have enum values");
    }

    [TestMethod]
    public void CatalogIncludesSeriesParameters()
    {
        // Act: get catalog with indicators that use series parameters
        IReadOnlyList<IndicatorListing> catalog = Catalog.Get();
        string reusable = $"{nameof(IReusable)}[]";

        // Find indicators that use ParamSeriesAttribute
        IndicatorListing prsListing = catalog.Single(x => x.Uiid == "PRS");
        IndicatorListing corrListing = catalog.Single(x => x.Uiid == "CORR");
        IndicatorListing betaListing = catalog.Single(x => x.Uiid == "BETA");

        // Assert: verify that series parameters have the TypeScript-friendly data type
        // PRS indicator
        prsListing.Parameters.Count.Should().BeGreaterThanOrEqualTo(2,
            "PRS should have at least two series parameters");
        prsListing.Parameters.Should().Contain(p =>
            p.ParamName == "sourceEval" &&
            p.DisplayName == "Evaluated Prices" &&
            p.DataType == "IReusable[]",
            "PRS should have a sourceEval parameter with IReusable[] data type");

        prsListing.Parameters.Should().Contain(p =>
            p.ParamName == "sourceBase" &&
            p.DisplayName == "Base Prices" &&
            p.DataType == "IReusable[]",
            "PRS should have a sourceBase parameter with IReusable[] data type");

        // Correlation indicator
        corrListing.Parameters.Count.Should().BeGreaterThanOrEqualTo(2,
            "CORR should have at least two series parameters");

        corrListing.Parameters.Should().Contain(p =>
            p.ParamName == "sourceA" &&
            p.DisplayName == "Source A" &&
            p.DataType == "IReusable[]",
            "CORR should have a sourceA parameter with IReusable[] data type");

        corrListing.Parameters.Should().Contain(p =>
            p.ParamName == "sourceB" &&
            p.DisplayName == "Source B" &&
            p.DataType == "IReusable[]",
            "CORR should have a sourceB parameter with IReusable[] data type");

        // Beta indicator
        betaListing.Parameters.Count.Should().BeGreaterThanOrEqualTo(2,
            "BETA should have at least two series parameters");

        betaListing.Parameters.Should().Contain(p =>
            p.ParamName == "sourceEval" &&
            p.DisplayName == "Evaluated Prices" &&
            p.DataType == "IReusable[]",
            "BETA should have a sourceEval parameter with IReusable[] data type");

        betaListing.Parameters.Should().Contain(p =>
            p.ParamName == "sourceMrkt" &&
            p.DisplayName == "Market Prices" &&
            p.DataType == "IReusable[]",
            "BETA should have a sourceMrkt parameter with IReusable[] data type");

        // Verify series parameters are excluded from legend templates
        prsListing.LegendTemplate.Should().Be("PRS([P3])",
            $"Series parameters should be excluded from PRS legend template");

        corrListing.LegendTemplate.Should().Be("CORR([P3])",
            $"Series parameters should be excluded from CORR legend template");

        betaListing.LegendTemplate.Should().Be("BETA([P3],[P4])",
            $"Series parameters should be excluded from BETA legend template");
    }
}
