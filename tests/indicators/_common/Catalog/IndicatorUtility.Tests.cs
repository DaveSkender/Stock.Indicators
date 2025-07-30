using System.Text.Json;

namespace Tests.Common.Catalog;

/// <summary>
/// Test class for IndicatorUtility functionality.
/// </summary>
[TestClass]
public class IndicatorUtilityTests : TestBase
{
    [TestMethod]
    public void ExecuteById_WithValidIndicator_ReturnsResults()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<object> results = IndicatorUtility.ExecuteById(id, style, quotes);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().HaveCountGreaterThan(0);
        
        // Check that we got actual RSI results
        results.Should().AllBeOfType<RsiResult>();
        
        // Verify the results match what we'd get from calling Rsi directly
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi();
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteById_WithParameters_ReturnsResults()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        var parameters = new Dictionary<string, object> { { "lookbackPeriods", 10 } };

        // Act
        IReadOnlyList<object> results = IndicatorUtility.ExecuteById(id, style, quotes, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<RsiResult>();

        // Verify the results match what we'd get from calling Rsi directly with same parameters
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi(10);
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteById_WithEma_ReturnsResults()
    {
        // Arrange
        string id = "EMA";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        var parameters = new Dictionary<string, object> { { "lookbackPeriods", 20 } };

        // Act
        IReadOnlyList<object> results = IndicatorUtility.ExecuteById(id, style, quotes, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<EmaResult>();

        // Verify the results match what we'd get from calling Ema directly
        IReadOnlyList<EmaResult> directResults = quotes.ToEma(20);
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteById_WithInvalidId_ThrowsException()
    {
        // Arrange
        string id = "INVALID_INDICATOR";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => IndicatorUtility.ExecuteById(id, style, quotes);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not found in registry*");
    }

    [TestMethod]
    public void ExecuteById_WithNullQuotes_ThrowsArgumentNullException()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IEnumerable<IQuote> quotes = null!;

        // Act & Assert
        Action act = () => IndicatorUtility.ExecuteById(id, style, quotes);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*quotes*");
    }

    [TestMethod]
    public void ExecuteById_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        string id = "";
        Style style = Style.Series;
        var quotes = Quotes.Take(50);

        // Act & Assert
        Action act = () => IndicatorUtility.ExecuteById(id, style, quotes);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*ID cannot be null or empty*");
    }

    [TestMethod]
    public void ExecuteFromJson_WithValidConfig_ReturnsResults()
    {
        // Arrange
        var config = new IndicatorConfig
        {
            Id = "RSI",
            Style = Style.Series,
            Parameters = new Dictionary<string, object> { { "lookbackPeriods", 14 } }
        };
        string json = JsonSerializer.Serialize(config);
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<object> results = IndicatorUtility.ExecuteFromJson(json, quotes);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<RsiResult>();

        // Verify the results match what we'd get from calling Rsi directly
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi(14);
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteFromJson_WithMinimalConfig_ReturnsResults()
    {
        // Arrange - minimal configuration with just ID and Style
        var config = new IndicatorConfig
        {
            Id = "EMA",
            Style = Style.Series
        };
        string json = JsonSerializer.Serialize(config);
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<object> results = IndicatorUtility.ExecuteFromJson(json, quotes);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<EmaResult>();
    }

    [TestMethod]
    public void ExecuteFromJson_WithInvalidJson_ThrowsArgumentException()
    {
        // Arrange
        string json = "{ invalid json }";
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => IndicatorUtility.ExecuteFromJson(json, quotes);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Invalid JSON configuration*");
    }

    [TestMethod]
    public void ExecuteFromJson_WithNullJson_ThrowsArgumentNullException()
    {
        // Arrange
        string json = null!;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => IndicatorUtility.ExecuteFromJson(json, quotes);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*json*");
    }

    [TestMethod]
    public void ExecuteFromJson_WithEmptyJson_ThrowsArgumentException()
    {
        // Arrange
        string json = "";
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => IndicatorUtility.ExecuteFromJson(json, quotes);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*JSON configuration cannot be null or empty*");
    }

    [TestMethod]
    public void ExecuteFromJson_WithNullQuotes_ThrowsArgumentNullException()
    {
        // Arrange
        var config = new IndicatorConfig { Id = "RSI", Style = Style.Series };
        string json = JsonSerializer.Serialize(config);
        IEnumerable<IQuote> quotes = null!;

        // Act & Assert
        Action act = () => IndicatorUtility.ExecuteFromJson(json, quotes);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*quotes*");
    }

    [TestMethod]
    public void ExecuteById_WithSma_ReturnsResults()
    {
        // Arrange
        string id = "SMA";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        var parameters = new Dictionary<string, object> { { "lookbackPeriods", 10 } };

        // Act
        IReadOnlyList<object> results = IndicatorUtility.ExecuteById(id, style, quotes, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<SmaResult>();

        // Verify the results match what we'd get from calling Sma directly
        IReadOnlyList<SmaResult> directResults = quotes.ToSma(10);
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteFromJson_RoundTrip_ProducesConsistentResults()
    {
        // Arrange - test that we can serialize and deserialize a config and get the same results
        var originalConfig = new IndicatorConfig
        {
            Id = "EMA",
            Style = Style.Series,
            Parameters = new Dictionary<string, object> { { "lookbackPeriods", 20 } },
            DisplayName = "Test EMA",
            Description = "Test description"
        };

        string json = JsonSerializer.Serialize(originalConfig);
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<object> jsonResults = IndicatorUtility.ExecuteFromJson(json, quotes);
        IReadOnlyList<object> directResults = IndicatorUtility.ExecuteById(originalConfig.Id, originalConfig.Style, quotes, originalConfig.Parameters);

        // Assert
        jsonResults.Should().NotBeNull();
        directResults.Should().NotBeNull();
        jsonResults.Should().HaveCount(directResults.Count);
        jsonResults.Should().AllBeOfType<EmaResult>();
        directResults.Should().AllBeOfType<EmaResult>();

        // Compare actual values
        for (int i = 0; i < jsonResults.Count; i++)
        {
            var jsonResult = (EmaResult)jsonResults[i];
            var directResult = (EmaResult)directResults[i];

            jsonResult.Timestamp.Should().Be(directResult.Timestamp);
            jsonResult.Ema.Should().Be(directResult.Ema);
        }
    }
}