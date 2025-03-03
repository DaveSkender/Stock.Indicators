using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Utilities;

[TestClass]
public class Metacatalogger
{
    private static readonly Uri BaseUrl = new("https://example.com");
    private const string TestCatalog = "_common/Metacatalog/listings.json";
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    [TestMethod]
    public void GeneratedCatalog()
    {


        // Act
        IReadOnlyList<IndicatorListing> result = Metacatalog.IndicatorCatalog();

        Action validate = () => Validator.ValidateObject(
            result,
            new ValidationContext(result),
            validateAllProperties: true);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(simpleCatalog);
        validate.Should().NotThrow<ValidationException>();

        Assert.Inconclusive("Generated catalog is not yet implemented.");
    }

    [TestMethod]
    public void ManualCatalog()
    {
        // Arrange
        string json = File.ReadAllText(TestCatalog);
        List<IndicatorListing> expectedListings = JsonSerializer.Deserialize<List<IndicatorListing>>(json, JsonOptions);

        // Create new instances with proper baseUrl constructor parameter
        string baseEndpoint = BaseUrl.ToString().TrimEnd('/');
        for (int i = 0; i < expectedListings.Count; i++)
        {
            IndicatorListing item = expectedListings[i];
            expectedListings[i] = new IndicatorListing(baseEndpoint) {
                Name = item.Name,
                Uiid = item.Uiid,
                UiidEndpoint = item.UiidEndpoint,
                Category = item.Category,
                ChartType = item.ChartType,
                ChartConfig = item.ChartConfig,
                LegendOverride = item.LegendOverride,
                Order = item.Order,
                Parameters = item.Parameters,
                Results = item.Results
            };
        }

        // Act
        IReadOnlyList<IndicatorListing> result = Metacatalog.IndicatorCatalog(BaseUrl);

        Action validate = () => Validator.ValidateObject(
            result,
            new ValidationContext(result),
            validateAllProperties: true);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(expectedListings);
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
    public void CatalogHasInvalidUiid()
    {
        // Arrange
        IndicatorListing indicator = new() {
            Name = "Test Indicator",
            Uiid = "ADX w/ FOO",
            Category = "test-category",
            ChartType = "overlay",
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

        // Act
        Action validate = () => Validator.ValidateObject(
            indicator,
            new ValidationContext(indicator),
            validateAllProperties: true);

        // Assert
        validate.Should()
            .Throw<ValidationException>()
            .WithMessage("The 'Uiid' field is not URL safe.");
    }

    private static readonly List<IndicatorListing> simpleCatalog = [

        // Exponential Moving Average (EMA)
        new IndicatorListing {
            Name = "Exponential Moving Average (EMA)",
            Uiid = "EMA",
            Category = "moving-average",
            ChartType = "overlay",
            Parameters = [
                new IndicatorParamConfig
                {
                    DisplayName = "Lookback Periods",
                    ParamName = "lookbackPeriods",
                    DataType = "int",
                    DefaultValue = 20,
                    Minimum = 2,
                    Maximum = 250
                }
            ],
            Results = [
                new IndicatorResultConfig
                {
                    DisplayName = "Exponential Moving Average",
                    TooltipTemplate = "EMA([P1])",
                    DataName = "ema",
                    DataType = "number",
                    LineType = "solid",
                    DefaultColor = ChartColors.StandardBlue
                }
            ]
        },

        // Simple Moving Average (SMA)
        new IndicatorListing {
            Name = "Simple Moving Average (SMA)",
            Uiid = "SMA",
            Category = "moving-average",
            ChartType = "overlay",
            Parameters = [
                new IndicatorParamConfig
                {
                    DisplayName = "Lookback Periods",
                    ParamName = "lookbackPeriods",
                    DataType = "int",
                    DefaultValue = 20,
                    Minimum = 2,
                    Maximum = 250
                }
            ],
            Results = [
                new IndicatorResultConfig
                {
                    DisplayName = "Simple Moving Average",
                    TooltipTemplate = "SMA([P1])",
                    DataName = "sma",
                    DataType = "number",
                    LineType = "solid",
                    DefaultColor = ChartColors.StandardBlue
                }
            ]
        }];
}
