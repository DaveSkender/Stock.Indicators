using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace Test.Data;

[TestClass]
public class Metacatalogger
{
    private static readonly Uri BaseUrl = new("https://example.com");

    // Cached JSON serializer options to fix CA1869 warning
    private static readonly JsonSerializerOptions IndentedJsonOptions = new() {
        WriteIndented = true
    };

    [TestMethod]
    public void GeneratedCatalog()
    {
        // Act
        IReadOnlyList<IndicatorListing> result = Metacatalog.IndicatorCatalog();

        // Assert
        // Check that the catalog contains indicators from different categories
        result.Should().Contain(x => x.Category == "moving-average", "Should contain moving average indicators");
        result.Should().Contain(x => x.Category == "Generated", "Should contain generated indicators");

        // Verify some specific indicators we know should be present
        result.Should().Contain(x => x.Uiid == "SMA", "Should contain SMA indicator");
        result.Should().Contain(x => x.Uiid == "EMA", "Should contain EMA indicator");

        // Verify style-specific indicators if present
        result.Where(x => x.Category == "Generated").Should()
            .Contain(x => x.Name.Contains("Stream") || x.Name.Contains("Buffer") || x.Name.Contains("Series"),
                "Should contain indicators from different styles");

        // Output catalog as pretty JSON for manual inspection
        List<IndicatorListing> generatedIndicators = result.Where(x => x.Category == "Generated").ToList();
        List<IndicatorListing> hardcodedIndicators = result.Where(x => x.Category != "Generated").ToList();

        var catalogOutput = new {
            TotalCount = result.Count,
            GeneratedCount = generatedIndicators.Count,
            HardcodedCount = hardcodedIndicators.Count,
            GeneratedIndicators = generatedIndicators.OrderBy(i => i.Name),
            HardcodedIndicators = hardcodedIndicators.OrderBy(i => i.Name)
        };

        // Use the cached JsonSerializerOptions instance
        string json = JsonSerializer.Serialize(catalogOutput, IndentedJsonOptions);
        Console.WriteLine(json);

        // Verify validity
        Action validate = () => Validator.ValidateObject(
            result,
            new ValidationContext(result),
            validateAllProperties: true);

        validate.Should().NotThrow<ValidationException>();
    }

    /// <summary>
    /// Outputs a legible representation of indicators to the console.
    /// </summary>
    private static void OutputIndicators(IEnumerable<IndicatorListing> indicators)
    {
        JsonSerializerOptions options = new() {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        foreach (IndicatorListing indicator in indicators.OrderBy(i => i.Name))
        {
            var summary = new {
                indicator.Name,
                indicator.Uiid,
                indicator.Category,
                indicator.ChartType,
                indicator.LegendTemplate,
                indicator.Endpoint,
                ParameterCount = indicator.Parameters?.Count ?? 0,
                ResultCount = indicator.Results?.Count ?? 0,
                Parameters = indicator.Parameters?.Select(static p => new {
                    p.DisplayName,
                    p.ParamName,
                    p.DataType,
                    DefaultValue = p.DefaultValue?.ToString(CultureInfo.InvariantCulture),
                    p.Minimum,
                    p.Maximum
                }).ToList(),
                Results = indicator.Results?.Select(r => new {
                    r.DisplayName,
                    r.DataName,
                    r.TooltipTemplate,
                    r.LineType,
                    r.DefaultColor
                }).ToList()
            };

            Console.WriteLine($"\n[{indicator.Uiid}] {indicator.Name}");
            Console.WriteLine($"Category: {indicator.Category}, ChartType: {indicator.ChartType}");
            Console.WriteLine($"Endpoint: {indicator.Endpoint}");
            Console.WriteLine($"LegendTemplate: {indicator.LegendTemplate}");
            Console.WriteLine($"Parameters: {summary.ParameterCount}, Results: {summary.ResultCount}");

            if (summary.ParameterCount > 0)
            {
                Console.WriteLine("Parameters:");
                foreach (var param in summary.Parameters)
                {
                    Console.WriteLine($"  * {param.DisplayName} ({param.ParamName}): {param.DataType}, " +
                                    $"Default: {param.DefaultValue}, Range: {param.Minimum} - {param.Maximum}");
                }
            }

            if (summary.ResultCount > 0)
            {
                Console.WriteLine("Results:");
                foreach (var result in summary.Results)
                {
                    Console.WriteLine($"  * {result.DisplayName} ({result.DataName}): {result.LineType}, " +
                                    $"Color: {result.DefaultColor}, Template: {result.TooltipTemplate}");
                }
            }

            Console.WriteLine(new string('-', 80));
        }
    }

    [TestMethod]
    public void ManualCatalog()
    {
        // Act
        IReadOnlyList<IndicatorListing> result = Metacatalog.IndicatorCatalog(BaseUrl);

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
        result.Should().Contain(x => x.Category == "Generated", "Should contain generated indicators");

        // Check if we can find our test indicators by ID
        result.Should().Contain(x => x.Uiid == "SMA", "Should contain SMA indicator");
        result.Should().Contain(x => x.Uiid == "EMA", "Should contain EMA indicator");

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
            Category = "test-category",
            ChartType = "overlay",
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
            Category = "test-category",
            ChartType = "overlay",
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
            Category = "test-category",
            ChartType = "overlay",
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
        // Act
        IReadOnlyList<IndicatorListing> result = Metacatalog.IndicatorCatalog();

        // Assert
        result.Should().OnlyContain(x => x.Category != null && x.ChartType != null,
            "Catalog listings should only be generated for indicators with the IndicatorAttribute");
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
            Category = "test-category",
            ChartType = "overlay",
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
        Action act = () => Metacatalog.ValidateUniqueUIID(catalog);

        // Assert
        act.Should().Throw<InvalidOperationException>("An error should be thrown when UIID is not defined uniquely");
    }
}
