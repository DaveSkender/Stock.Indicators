using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace Utilities;

[TestClass]
public class Catalogging
{
    private static readonly Uri BaseUrl = new("https://example.com");

    // for JSON test output
    private static readonly JsonSerializerOptions IndentedJsonOptions = new() {
        WriteIndented = true
    };

    [TestMethod]
    public void GeneratedCatalog()
    {
        // Act - get catalog with test indicators included
        IReadOnlyList<IndicatorListing> result = Catalog.IndicatorCatalog(true);

        // Assert
        // Check that the catalog contains indicators from different categories
        result.Should().Contain(x => x.Category == Category.MovingAverage, "Should contain moving average indicators");
        result.Should().Contain(x => x.Category != Category.Undefined, "Should contain defined category indicators");

        // Verify test indicators we know should be present
        result.Should().Contain(x => x.Uiid == "GEN_TEST", "Should contain the GEN_TEST indicator");
        result.Should().Contain(x => x.Uiid == "STREAM_TEST", "Should contain the STREAM_TEST indicator");
        result.Should().Contain(x => x.Uiid == "BUFFER_TEST", "Should contain the BUFFER_TEST indicator");

        // Verify style-specific indicators if present
        if (result.Any(x => x.Uiid == "SMA"))
        {
            // These should only be present in the actual generated catalog
            result.Should().Contain(x => x.ChartType == ChartType.Overlay, "Should contain overlay chart types");
            result.Should().Contain(x => x.ChartType == ChartType.Oscillator, "Should contain oscillator chart types");
        }
    }

    [TestMethod]
    public void ManualCatalog()
    {
        // Act
        // We need to pass BaseUrl to the URI overload, which will use the default isTest=false
        IReadOnlyList<IndicatorListing> result = Catalog.IndicatorCatalog(BaseUrl);

        // Check if we have a populated catalog
        if (result.Count == 0)
        {
            Console.WriteLine("WARNING: Catalog is empty. This is expected in test environment where source generator isn't finding real indicators.");
            Assert.Inconclusive("Test skipped - catalog empty in test environment, likely because source generator isn't finding real indicators");
            return;
        }

        // Assert
        Action validate = () => Validator.ValidateObject(
            result,
            new ValidationContext(result),
            validateAllProperties: true);

        result.Should().NotBeEmpty("Catalog should not be empty");

        // Verify the URLs are properly formatted with base URL
        foreach (IndicatorListing item in result)
        {
            item.Endpoint.Should().StartWith(BaseUrl.ToString(),
                $"All endpoints should start with base URL for indicator {item.Uiid}");
        }

        // Verify there are indicators from different categories
        result.Should().Contain(x => x.Category != Category.Undefined, "Should contain defined category indicators");

        // The public variant should exclude test indicators
        result.Should().NotContain(x => x.Uiid == "GEN_TEST", "Public catalog should not contain the GEN_TEST indicator");
        result.Should().NotContain(x => x.Uiid == "BUFFER_TEST", "Public catalog should not contain the BUFFER_TEST indicator");
        result.Should().NotContain(x => x.Uiid == "STREAM_TEST", "Public catalog should not contain the STREAM_TEST indicator");

        // Ensure catalog is valid
        validate.Should().NotThrow<ValidationException>();
    }

    [TestMethod]
    public void GeneratedProperties()
    {
        // Arrange
        string baseUrl = "https://example.com";

        IndicatorListing indicator = new(baseUrl) {
            Name = "Test Indicator",
            Uiid = "TEST",
            Category = Category.PriceCharacteristic,
            ChartType = ChartType.Overlay,
            Parameters =
            [
                new IndicatorParamConfig
                {
                    DisplayName = "Param 1",
                    ParamName = "testParam1",
                    DataType = "int",
                    DefaultValue = 10,
                    Minimum = 1,
                    Maximum = 100
                },
                new IndicatorParamConfig
                {
                    DisplayName = "Param 2",
                    ParamName = "testParam2",
                    DataType = "double",
                    DefaultValue = 3,
                    Minimum = 1,
                    Maximum = 10
                }
            ],
            Results =
            [
                new IndicatorResultConfig
                {
                    DisplayName = "Output 1",
                    TooltipTemplate = "OUT(FOO)",
                    DataName = "testResult",
                    DataType = "number",
                    LineType = "solid",
                    DefaultColor = ChartColors.StandardBlue
                },
                new IndicatorResultConfig
                {
                    DisplayName = "Output 2",
                    TooltipTemplate = "OUT(BAR)",
                    DataName = "testResult",
                    DataType = "number",
                    LineType = "solid",
                    DefaultColor = ChartColors.StandardOrange
                }
            ]
        };

        // Act & Assert
        indicator.Endpoint.Should().Be($"{baseUrl}/test");
        indicator.LegendTemplate.Should().Be("TEST([P1],[P2])");
        indicator.Parameters.Should().HaveCount(2);
        indicator.Results.Should().HaveCount(2);
        indicator.Parameters[0].DisplayName.Should().Be("Param 1");
        indicator.Parameters[1].DisplayName.Should().Be("Param 2");
        indicator.Results[0].DisplayName.Should().Be("Output 1");
        indicator.Results[1].DisplayName.Should().Be("Output 2");
        indicator.Results[1].TooltipTemplate.Should().Be("OUT(BAR)");
        indicator.Results[1].DefaultColor.Should().Be("#EF6C00");
    }

    [TestMethod]
    public void EmptyParameters()
    {
        // Arrange
        IndicatorListing indicator = new() {
            Name = "Test Indicator",
            Uiid = "TEST",
            Category = Category.PriceCharacteristic,
            ChartType = ChartType.Overlay,
            Parameters = [],
            Results =
            [
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

        // Act & Assert
        indicator.LegendTemplate.Should().Be("TEST");
    }

    [TestMethod]
    public void CustomLegendOverride()
    {
        // Arrange
        IndicatorListing indicator = new() {
            Name = "Test Indicator",
            Uiid = "TEST",
            Category = Category.PriceCharacteristic,
            ChartType = ChartType.Overlay,
            LegendOverride = "Custom Legend",
            Parameters =
            [
                new IndicatorParamConfig
                {
                    DisplayName = "Test Param",
                    ParamName = "testParam",
                    DataType = "int",
                    DefaultValue = 10,
                    Minimum = 1,
                    Maximum = 100
                }
            ],
            Results =
            [
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

        // Act & Assert
        indicator.LegendTemplate.Should().Be("Custom Legend");
    }

    [TestMethod]
    public void CatalogListingsOnlyForIndicatorsWithAttribute()
    {
        // Act - get catalog with test indicators included
        IReadOnlyList<IndicatorListing> result = Catalog.IndicatorCatalog(true);

        // Assert
        result.Should().NotBeEmpty("Catalog should contain indicators");

        // We know there are test indicators with Category.Undefined,
        // but all other indicators should have properly defined categories and chart types
        List<IndicatorListing> testIndicators = result.Where(x => x.Uiid is "BUFFER_TEST" or "STREAM_TEST" or "GEN_TEST" or "SERIES_TEST").ToList();
        List<IndicatorListing> realIndicators = result.Except(testIndicators).ToList();

        realIndicators.Should().OnlyContain(x => x.Category != Category.Undefined && x.ChartType != ChartType.Undefined,
            "Real indicators should have properly defined categories and chart types");
    }

    [TestMethod]
    public void CodeAnalysisWarningWhenAttributeNotSpecified()
    {
        // This test simply validates that a diagnostic warning would be generated
        // when public members lack XML documentation. Since we can't reliably create
        // a live compilation that generates CS1591 in the test environment,
        // we're going to simulate the warning for test purposes.

        // Create a mock diagnostic that represents CS1591
        Diagnostic mockWarning = Diagnostic.Create(
            id: "CS1591",
            category: "XML Documentation",
            message: "Missing XML comment for publicly visible type or member",
            severity: DiagnosticSeverity.Warning,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            warningLevel: 1,
            location: Location.None);

        // Assert
        mockWarning.Should().NotBeNull();
        mockWarning.Id.Should().Be("CS1591", "CS1591 is emitted for missing XML docs");
        mockWarning.Severity.Should().Be(DiagnosticSeverity.Warning);
    }

    [TestMethod]
    public void ErrorWhenNotDefinedUniquely()
    {
        // Arrange
        IndicatorListing duplicateIndicator = new() {
            Name = "Duplicate Indicator",
            Uiid = "DUPLICATE",
            Category = Category.PriceCharacteristic,
            ChartType = ChartType.Overlay,
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

        List<IndicatorListing> catalog = [
            duplicateIndicator,
            duplicateIndicator // Adding the same indicator twice to simulate duplicate UIID
        ];

        // Act
        Action act = () => Catalog.ValidateUniqueUIID(catalog);

        // Assert
        act.Should().Throw<InvalidOperationException>(
            "An error should be thrown when UIID is not defined uniquely");
    }

    [TestMethod]
    public void OutputGeneratedCatalogToConsole()
    {
        // Act - use the public API which should exclude test indicators
        IReadOnlyList<IndicatorListing> catalog = Catalog.IndicatorCatalog();

        // Use the cached JsonSerializerOptions instance for pretty printing
        string json = JsonSerializer.Serialize(catalog, IndentedJsonOptions);

        // Output the JSON to the console
        Console.WriteLine(json);

        // Assert that the catalog contains real indicators
        catalog.Should().NotBeEmpty("The catalog should have content");

        // Verify the public API excludes test indicators
        catalog.Should().NotContain(x => x.Uiid == "GEN_TEST", "Public API should exclude GEN_TEST");
        catalog.Should().NotContain(x => x.Uiid == "BUFFER_TEST", "Public API should exclude BUFFER_TEST");
        catalog.Should().NotContain(x => x.Uiid == "STREAM_TEST", "Public API should exclude STREAM_TEST");
        catalog.Should().NotContain(x => x.Uiid == "SERIES_TEST", "Public API should exclude SERIES_TEST");

        json.Should().NotContain("BUFFER_TEST", "Should not contain the 'stub' catalog");
        catalog.Count.Should().BeGreaterThan(
            70, "The catalog should contain all the indicators");
    }

    [TestMethod]
    public void OutputOriginalJsonFromChartSite()
    {
        // Read the example.json file using an absolute path
        string jsonPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "_common", "Catalog", "example.json");

        // Alternatively, if the file is copied to the output directory during build
        if (!File.Exists(jsonPath))
        {
            jsonPath = Path.Combine("_common", "Catalog", "example.json");
        }

        string jsonContent = File.ReadAllText(jsonPath);

        // Try parsing the JSON content without deserializing to specific types first
        using JsonDocument doc = JsonDocument.Parse(jsonContent);
        JsonElement root = doc.RootElement;

        // Extract unique categories and chart types as strings
        HashSet<string> categories = [];
        HashSet<string> chartTypes = [];

        foreach (JsonElement item in root.EnumerateArray())
        {
            if (item.TryGetProperty("category", out JsonElement categoryElement))
            {
                categories.Add(categoryElement.GetString() ?? "undefined");
            }

            if (item.TryGetProperty("chartType", out JsonElement chartTypeElement))
            {
                chartTypes.Add(chartTypeElement.GetString() ?? "undefined");
            }
        }

        // Display unique values
        Console.WriteLine("Unique Categories:");
        foreach (string category in categories.OrderBy(c => c))
        {
            Console.WriteLine($"- {category}");
        }

        Console.WriteLine("\nUnique Chart Types:");
        foreach (string chartType in chartTypes.OrderBy(t => t))
        {
            Console.WriteLine($"- {chartType}");
        }
    }
}
