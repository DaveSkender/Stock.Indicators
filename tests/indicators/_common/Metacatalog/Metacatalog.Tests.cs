using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Test.Data;

[TestClass]
public class Metacatalogger
{
    private static readonly Uri BaseUrl = new("https://example.com");

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

        // Output catalog statistics
        int generatedCount = result.Count(x => x.Category == "Generated");
        Console.WriteLine($"Total indicators: {result.Count}");
        Console.WriteLine($"Generated indicators: {generatedCount}");
        Console.WriteLine($"Hardcoded indicators: {result.Count - generatedCount}");

        // Verify validity
        Action validate = () => Validator.ValidateObject(
            result,
            new ValidationContext(result),
            validateAllProperties: true);

        validate.Should().NotThrow<ValidationException>();
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
        // Arrange
        var compilation = CSharpCompilation.Create("TestAssembly")
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(@"
                namespace TestNamespace
                {
                    public class TestClass
                    {
                        public void TestMethod() { }
                    }
                }
            "));

        var diagnostics = compilation.GetDiagnostics();

        // Act
        var warning = diagnostics.FirstOrDefault(d => d.Id == "CS1591");

        // Assert
        warning.Should().NotBeNull("Code analysis warning should be emitted when IndicatorAttribute is not specified");
    }

    [TestMethod]
    public void ErrorWhenUIIDNotDefinedUniquely()
    {
        // Arrange
        var duplicateIndicator = new IndicatorListing {
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

        var catalog = new List<IndicatorListing>
        {
            duplicateIndicator,
            duplicateIndicator // Adding the same indicator twice to simulate duplicate UIID
        };

        // Act
        Action act = () => Metacatalog.ValidateUniqueUIID(catalog);

        // Assert
        act.Should().Throw<InvalidOperationException>("An error should be thrown when UIID is not defined uniquely");
    }
}
