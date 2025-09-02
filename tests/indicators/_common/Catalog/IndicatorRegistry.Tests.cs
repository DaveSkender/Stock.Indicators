using System.Reflection;
#nullable enable

namespace Catalog;

[TestClass]
[DoNotParallelize]
public class CatalogRegistry : TestBase
{
    [TestInitialize]
    public void Setup() =>
        // Clear the registry before each test
        IndicatorRegistry.Clear();

    [TestCleanup]
    public void Cleanup()
    {
        // Clear the registry after each test
        IndicatorRegistry.Clear();
        // Re-enable auto-initialization for other tests
        IndicatorRegistry.EnableAutoInitialization();
    }

    [TestMethod]
    public void RegisterValidListingShouldSucceed()
    {
        // Arrange
        IndicatorListing listing = CreateTestListing("TEST", "Test Indicator");

        // Act
        IndicatorRegistry.Register(listing);

        // Assert
        IndicatorListing? retrieved = IndicatorRegistry.GetIndicator("TEST");
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Test Indicator");
        retrieved.Uiid.Should().Be("TEST");
    }

    [TestMethod]
    public void RegisterNullListingShouldThrowArgumentNullException() =>
        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => IndicatorRegistry.Register(null!));

    [TestMethod]
    public void RegisterDuplicateUiidShouldThrowInvalidOperationException()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListing("TEST", "Test Indicator 1");
        IndicatorListing listing2 = CreateTestListing("TEST", "Test Indicator 2");

        // Act
        IndicatorRegistry.Register(listing1);

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => IndicatorRegistry.Register(listing2));
    }

    [TestMethod]
    public void RegisterCaseInsensitiveUiidShouldThrowInvalidOperationException()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListing("TEST", "Test Indicator 1");
        IndicatorListing listing2 = CreateTestListing("test", "Test Indicator 2");

        // Act
        IndicatorRegistry.Register(listing1);

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(() => IndicatorRegistry.Register(listing2));
    }

    [TestMethod]
    public void GetIndicatorExistingUiidShouldReturnListing()
    {
        // Arrange
        IndicatorListing listing = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorRegistry.Register(listing);

        // Act
        IndicatorListing? result = IndicatorRegistry.GetIndicator("EMA");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Exponential Moving Average");
        result.Uiid.Should().Be("EMA");
    }

    [TestMethod]
    public void GetIndicatorCaseInsensitiveUiidShouldReturnListing()
    {
        // Arrange
        IndicatorListing listing = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorRegistry.Register(listing);

        // Act
        IndicatorListing? result = IndicatorRegistry.GetIndicator("ema");

        // Assert
        result.Should().NotBeNull();
        result!.Uiid.Should().Be("EMA");
    }

    [TestMethod]
    public void GetIndicatorNonExistentUiidShouldReturnNull()
    {
        // Act
        IndicatorListing? result = IndicatorRegistry.GetIndicator("NONEXISTENT");

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void GetIndicatorNullOrWhitespaceUiidShouldReturnNull()
    {
        // Act & Assert
        IndicatorRegistry.GetIndicator(null!).Should().BeNull();
        IndicatorRegistry.GetIndicator("").Should().BeNull();
        IndicatorRegistry.GetIndicator("   ").Should().BeNull();
    }

    [TestMethod]
    public void GetAllIndicatorsEmptyRegistryShouldReturnEmptyCollection()
    {
        // Arrange - make sure the registry is empty
        IndicatorRegistry.Clear();

        // Act
        IReadOnlyCollection<IndicatorListing> indicators = IndicatorRegistry.GetIndicators();

        // Assert
        indicators.Should().NotBeNull();
        indicators.Should().BeEmpty();
    }

    [TestMethod]
    public void GetAllIndicatorsWithMultipleIndicatorsShouldReturnAllRegistered()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorListing listing2 = CreateTestListing("SMA", "Simple Moving Average");
        IndicatorListing listing3 = CreateTestListing("MACD", "Moving Average Convergence Divergence");

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> indicators = IndicatorRegistry.GetIndicators();

        // Assert
        indicators.Should().NotBeNull();
        indicators.Should().HaveCount(3);
        indicators.Should().Contain(l => l.Uiid == "EMA");
        indicators.Should().Contain(l => l.Uiid == "SMA");
        indicators.Should().Contain(l => l.Uiid == "MACD");
    }

    [TestMethod]
    public void GetAllIndicatorsShouldReturnReadOnlyCollection()
    {
        // Arrange
        IndicatorListing listing = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorRegistry.Register(listing);

        // Act
        IReadOnlyCollection<IndicatorListing> indicators = IndicatorRegistry.GetIndicators();

        // Assert
        indicators.Should().BeAssignableTo<IReadOnlyCollection<IndicatorListing>>();
    }

    [TestMethod]
    public void RegisterCatalogMultipleAssembliesShouldHandleGracefully()
    {
        // Arrange
        Assembly[] assemblies = [
            typeof(IndicatorRegistry).Assembly,
            typeof(CatalogRegistry).Assembly  // tests
        ];

        // Act & Assert - Should not throw
        IndicatorRegistry.RegisterCatalog(assemblies);
    }

    // New Catalog API Tests
    [TestMethod]
    public void GetCatalogWithoutFilterShouldReturnAllIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithStyle("EMA", "Exponential Moving Average", Style.Series);
        IndicatorListing listing2 = CreateTestListingWithStyle("SMA", "Simple Moving Average", Style.Stream);
        IndicatorListing listing3 = CreateTestListingWithStyle("MACD", "Moving Average Convergence Divergence", Style.Buffer);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> catalog = IndicatorRegistry.GetCatalog();

        // Assert
        catalog.Should().NotBeNull();
        catalog.Should().HaveCount(3);
        catalog.Should().Contain(l => l.Uiid == "EMA");
        catalog.Should().Contain(l => l.Uiid == "SMA");
        catalog.Should().Contain(l => l.Uiid == "MACD");
    }

    [TestMethod]
    public void GetCatalogWithStyleFilterShouldReturnFilteredIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithStyle("EMA", "Exponential Moving Average", Style.Series);
        IndicatorListing listing2 = CreateTestListingWithStyle("SMA", "Simple Moving Average", Style.Stream);
        IndicatorListing listing3 = CreateTestListingWithStyle("RSI", "Relative Strength Index", Style.Series);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> seriesIndicators = IndicatorRegistry.GetCatalog(Style.Series);

        // Assert
        seriesIndicators.Should().HaveCount(2);
        seriesIndicators.Should().Contain(l => l.Uiid == "EMA");
        seriesIndicators.Should().Contain(l => l.Uiid == "RSI");
        seriesIndicators.Should().NotContain(l => l.Uiid == "SMA");
    }

    [TestMethod]
    public void SearchWithValidTermShouldReturnMatchingIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorListing listing2 = CreateTestListing("SMA", "Simple Moving Average");
        IndicatorListing listing3 = CreateTestListing("RSI", "Relative Strength Index");

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> results = IndicatorRegistry.Search("Moving");

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(l => l.Uiid == "EMA");
        results.Should().Contain(l => l.Uiid == "SMA");
        results.Should().NotContain(l => l.Uiid == "RSI");
    }

    [TestMethod]
    public void SearchCaseInsensitiveShouldReturnMatchingIndicators()
    {
        // Arrange
        IndicatorListing listing = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorRegistry.Register(listing);

        // Act
        IReadOnlyCollection<IndicatorListing> results = IndicatorRegistry.Search("exponential");

        // Assert
        results.Should().HaveCount(1);
        results.Should().Contain(l => l.Uiid == "EMA");
    }

    [TestMethod]
    public void SearchWithEmptyTermShouldReturnAllIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorListing listing2 = CreateTestListing("SMA", "Simple Moving Average");

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);

        // Act
        IReadOnlyCollection<IndicatorListing> results = IndicatorRegistry.Search("");

        // Assert
        results.Should().HaveCount(2);
    }

    [TestMethod]
    public void GetByCategoryShouldReturnIndicatorsInCategory()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithCategory("EMA", "Exponential Moving Average", Category.MovingAverage);
        IndicatorListing listing2 = CreateTestListingWithCategory("RSI", "Relative Strength Index", Category.Oscillator);
        IndicatorListing listing3 = CreateTestListingWithCategory("SMA", "Simple Moving Average", Category.MovingAverage);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> movingAverages = IndicatorRegistry.GetByCategory(Category.MovingAverage);

        // Assert
        movingAverages.Should().HaveCount(2);
        movingAverages.Should().Contain(l => l.Uiid == "EMA");
        movingAverages.Should().Contain(l => l.Uiid == "SMA");
        movingAverages.Should().NotContain(l => l.Uiid == "RSI");
    }

    [TestMethod]
    public void GetByStyleShouldReturnIndicatorsWithStyle()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListingWithStyle("EMA", "Exponential Moving Average", Style.Series);
        IndicatorListing listing2 = CreateTestListingWithStyle("SMA", "Simple Moving Average", Style.Stream);
        IndicatorListing listing3 = CreateTestListingWithStyle("RSI", "Relative Strength Index", Style.Series);

        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);
        IndicatorRegistry.Register(listing3);

        // Act
        IReadOnlyCollection<IndicatorListing> seriesIndicators = IndicatorRegistry.GetByStyle(Style.Series);

        // Assert
        seriesIndicators.Should().HaveCount(2);
        seriesIndicators.Should().Contain(l => l.Uiid == "EMA");
        seriesIndicators.Should().Contain(l => l.Uiid == "RSI");
        seriesIndicators.Should().NotContain(l => l.Uiid == "SMA");
    }

    [TestMethod]
    public void ThreadSafetyConcurrentRegistrationShouldHandleCorrectly()
    {
        // Arrange
        const int threadCount = 10;
        const int itemsPerThread = 10;
        List<Task> tasks = [];
        List<Exception> exceptions = [];

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            int threadIndex = i;
            tasks.Add(Task.Run(() => {
                try
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        IndicatorListing listing = CreateTestListing(
                            $"TEST-{threadIndex}-{j}", $"Test Indicator {threadIndex}-{j}");

                        IndicatorRegistry.Register(listing);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        exceptions.Should().BeEmpty("All registrations should succeed with unique UIIDs");

        IReadOnlyCollection<IndicatorListing> indicators = IndicatorRegistry.GetIndicators();
        indicators.Should().HaveCount(threadCount * itemsPerThread);
    }

    [TestMethod]
    public void ThreadSafetyConcurrentAccessShouldReturnConsistentResults()
    {
        // Arrange
        IndicatorListing listing = CreateTestListing("CONCURRENT", "Concurrent Test Indicator");
        IndicatorRegistry.Register(listing);

        const int threadCount = 20;
        List<Task<IndicatorListing?>> tasks = [];

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() => IndicatorRegistry.GetIndicator("CONCURRENT")));
        }

        IndicatorListing?[] results = Task.WhenAll(tasks).Result;

        // Assert
        results.Should().AllSatisfy(result => {
            result.Should().NotBeNull();
            result!.Uiid.Should().Be("CONCURRENT");
            result.Name.Should().Be("Concurrent Test Indicator");
        });
    }

    [TestMethod]
    public void ClearShouldRemoveAllRegisteredIndicators()
    {
        // Arrange
        IndicatorListing listing1 = CreateTestListing("EMA", "Exponential Moving Average");
        IndicatorListing listing2 = CreateTestListing("SMA", "Simple Moving Average");
        IndicatorRegistry.Register(listing1);
        IndicatorRegistry.Register(listing2);

        // Verify indicators are registered
        IndicatorRegistry.GetIndicators().Should().HaveCount(2);

        // Act
        IndicatorRegistry.Clear();

        // Assert
        // We need to use reflection to call the internal GetIndicatorWithoutInitialization method
        // to avoid triggering auto-initialization
        MethodInfo? method = typeof(IndicatorRegistry)
            .GetMethod(
                name: "GetIndicatorWithoutInitialization",
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Static);

        if (method != null)
        {
            object? ema = method.Invoke(null, ["EMA"]);
            object? sma = method.Invoke(null, ["SMA"]);

            ema.Should().BeNull();
            sma.Should().BeNull();
        }
        else
        {
            // Fallback assertion if the method is not found
            IndicatorRegistry.GetIndicators().Should().BeEmpty();
        }
    }

    private static IndicatorListing CreateTestListing(string uiid, string name)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Result", "Result", ResultType.Default, isReusable: true)
            .Build();

    private static IndicatorListing CreateTestListingWithStyle(string uiid, string name, Style style)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(style)
            .WithCategory(Category.Undefined)
            .AddResult("Result", "Result", ResultType.Default, isReusable: true)
            .Build();

    private static IndicatorListing CreateTestListingWithCategory(string uiid, string name, Category category)
        => new IndicatorListingBuilder()
            .WithName(name)
            .WithId(uiid)
            .WithStyle(Style.Series)
            .WithCategory(category)
            .AddResult("Result", "Result", ResultType.Default, isReusable: true)
            .Build();
}
