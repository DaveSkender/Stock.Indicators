#nullable enable
using Skender.Stock.Indicators;

namespace Catalog;

[TestClass]
[DoNotParallelize]
public class CatalogRegistryExtensions : TestBase
{
    [TestInitialize]
    public void Setup() =>
        // Clear the registry before each test
        IndicatorRegistry.Clear();

    [TestCleanup]
    public void Cleanup() =>
        // Clear the registry after each test
        IndicatorRegistry.Clear();

    [TestMethod]
    public void SearchByNameWithStyleShouldReturnCorrectIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithStyle("EMA", "Exponential Moving Average", Style.Series);
        IndicatorListing listing2 = CreateTestListingWithStyle("SMA", "Simple Moving Average", Style.Series);
        IndicatorListing listing3 = CreateTestListingWithStyle("MACD", "Moving Average Convergence Divergence", Style.Buffer);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.SearchByName("Moving", Style.Series);

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(l => l.Uiid == "EMA");
        results.Should().Contain(l => l.Uiid == "SMA");
        results.Should().NotContain(l => l.Uiid == "MACD");
    }

    [TestMethod]
    public void SearchByNameWithCategoryShouldReturnCorrectIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithCategory("EMA", "Exponential Moving Average", Category.MovingAverage);
        IndicatorListing listing2 = CreateTestListingWithCategory("SMA", "Simple Moving Average", Category.MovingAverage);
        IndicatorListing listing3 = CreateTestListingWithCategory("RSI", "Relative Strength Index", Category.Oscillator);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.SearchByName("Moving", Category.MovingAverage);

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(l => l.Uiid == "EMA");
        results.Should().Contain(l => l.Uiid == "SMA");
        results.Should().NotContain(l => l.Uiid == "RSI");
    }

    [TestMethod]
    public void GetIndicatorsWithResultTypeShouldReturnCorrectIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithResultType("EMA", "Exponential Moving Average", ResultType.Default);
        IndicatorListing listing2 = CreateTestListingWithResultType("SMA", "Simple Moving Average", ResultType.Channel);
        IndicatorListing listing3 = CreateTestListingWithResultType("RSI", "Relative Strength Index", ResultType.Default);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.GetIndicatorsWithResultType(ResultType.Default);

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(l => l.Uiid == "EMA");
        results.Should().Contain(l => l.Uiid == "RSI");
        results.Should().NotContain(l => l.Uiid == "SMA");
    }

    [TestMethod]
    public void GetIndicatorsWithResultNameShouldReturnCorrectIndicators()
    {
        // Clear registry to ensure we have a clean state
        IndicatorRegistry.Clear();

        // Arrange
        IndicatorListing listing1 = CreateTestListingWithResultName("EMA", "Exponential Moving Average", "EmaResult");
        IndicatorListing listing2 = CreateTestListingWithResultName("SMA", "Simple Moving Average", "SmaValue");
        IndicatorListing listing3 = CreateTestListingWithResultName("RSI", "Relative Strength Index", "RsiValue");

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.GetIndicatorsWithResultName("Value");

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(l => l.Uiid == "SMA");
        results.Should().Contain(l => l.Uiid == "RSI");
        results.Should().NotContain(l => l.Uiid == "EMA");
    }

    [TestMethod]
    public void GetIndicatorsSortedByNameShouldReturnSortedIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorListing listing2 = CreateTestListing("SMA", "Simple Moving Average");
        IndicatorListing listing3 = CreateTestListing("MACD", "Moving Average Convergence Divergence");

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act - Ascending
        IReadOnlyCollection<IndicatorListing> ascendingResults
            = IndicatorRegistryExtensions.GetIndicatorsSortedByName();

        // Assert - Ascending
        ascendingResults.Should().HaveCount(3);
        List<IndicatorListing> ascendingList = ascendingResults.ToList();
        ascendingList[0].Name.Should().Be("Exponential Moving Average");
        ascendingList[1].Name.Should().Be("Moving Average Convergence Divergence");
        ascendingList[2].Name.Should().Be("Simple Moving Average");

        // Act - Descending
        IReadOnlyCollection<IndicatorListing> descendingResults
            = IndicatorRegistryExtensions.GetIndicatorsSortedByName(ascending: false);

        // Assert - Descending
        descendingResults.Should().HaveCount(3);
        List<IndicatorListing> descendingList = descendingResults.ToList();
        descendingList[0].Name.Should().Be("Simple Moving Average");
        descendingList[1].Name.Should().Be("Moving Average Convergence Divergence");
        descendingList[2].Name.Should().Be("Exponential Moving Average");
    }

    [TestMethod]
    public void GetIndicatorsWithRequiredParametersShouldReturnCorrectIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithRequiredParam("EMA", "Exponential Moving Average", true);
        IndicatorListing listing2 = CreateTestListingWithRequiredParam("SMA", "Simple Moving Average", false);
        IndicatorListing listing3 = CreateTestListingWithRequiredParam("RSI", "Relative Strength Index", true);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.GetIndicatorsWithRequiredParameters();

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(l => l.Uiid == "EMA");
        results.Should().Contain(l => l.Uiid == "RSI");
        results.Should().NotContain(l => l.Uiid == "SMA");
    }

    [TestMethod]
    public void GetIndicatorsWithOptionalParametersShouldReturnCorrectIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithRequiredParam("EMA", "Exponential Moving Average", true);
        IndicatorListing listing2 = CreateTestListingWithRequiredParam("SMA", "Simple Moving Average", false);
        IndicatorListing listing3 = CreateTestListingWithRequiredParam("RSI", "Relative Strength Index", true);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.GetIndicatorsWithOptionalParameters();

        // Assert
        results.Should().HaveCount(1);
        results.Should().Contain(l => l.Uiid == "SMA");
        results.Should().NotContain(l => l.Uiid == "EMA");
        results.Should().NotContain(l => l.Uiid == "RSI");
    }

    private static IndicatorListing CreateTestListing(string uiid, string name)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Result", "Result", ResultType.Default, isDefault: true)
            .Build();

    private static IndicatorListing CreateTestListingWithStyle(string uiid, string name, Style style)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(style)
            .WithCategory(Category.Undefined)
            .AddResult("Result", "Result", ResultType.Default, isDefault: true)
            .Build();

    private static IndicatorListing CreateTestListingWithCategory(string uiid, string name, Category category)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(Style.Series)
            .WithCategory(category)
            .AddResult("Result", "Result", ResultType.Default, isDefault: true)
            .Build();

    private static IndicatorListing CreateTestListingWithResultType(string uiid, string name, ResultType resultType)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Result", "Result", resultType, isDefault: true)
            .Build();

    private static IndicatorListing CreateTestListingWithResultName(string uiid, string name, string resultName)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult(resultName, resultName, ResultType.Default, isDefault: true)
            .Build();

    private static IndicatorListing CreateTestListingWithRequiredParam(string uiid, string name, bool isRequired)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddParameter<int>("period", "Period", "Test description", isRequired)
            .AddResult("Result", "Result", ResultType.Default, isDefault: true)
            .Build();
}
