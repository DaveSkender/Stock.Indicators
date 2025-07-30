using System.Text.Json;

namespace Tests.Common.Catalog;

/// <summary>
/// Test class for CatalogUtility functionality.
/// </summary>
[TestClass]
public class CatalogUtilityTests : TestBase
{
    [TestMethod]
    public void ExecuteByIdWithValidIndicatorReturnsResults()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<object> results = quotes.ExecuteById(id, style);

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
    public void ExecuteByIdWithParametersReturnsResults()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        var parameters = new Dictionary<string, object> { { "lookbackPeriods", 10 } };

        // Act
        IReadOnlyList<object> results = quotes.ExecuteById(id, style, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<RsiResult>();

        // Verify the results match what we'd get from calling Rsi directly with same parameters
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi(10);
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteByIdWithEmaReturnsResults()
    {
        // Arrange
        string id = "EMA";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        var parameters = new Dictionary<string, object> { { "lookbackPeriods", 20 } };

        // Act
        IReadOnlyList<object> results = quotes.ExecuteById(id, style, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<EmaResult>();

        // Verify the results match what we'd get from calling Ema directly
        IReadOnlyList<EmaResult> directResults = quotes.ToEma(20);
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteByIdWithInvalidIdThrowsException()
    {
        // Arrange
        string id = "INVALID_INDICATOR";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteById(id, style);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not found in registry*");
    }

    [TestMethod]
    public void ExecuteByIdWithNullQuotesThrowsArgumentNullException()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IEnumerable<IQuote> quotes = null!;

        // Act & Assert
        Action act = () => quotes.ExecuteById(id, style);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*quotes*");
    }

    [TestMethod]
    public void ExecuteByIdWithEmptyIdThrowsArgumentException()
    {
        // Arrange
        string id = "";
        Style style = Style.Series;
        var quotes = Quotes.Take(50);

        // Act & Assert
        Action act = () => quotes.ExecuteById(id, style);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*ID cannot be null or empty*");
    }

    [TestMethod]
    public void ExecuteFromJsonWithValidConfigReturnsResults()
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
        IReadOnlyList<object> results = quotes.ExecuteFromJson(json);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<RsiResult>();

        // Verify the results match what we'd get from calling Rsi directly
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi(14);
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteFromJsonWithMinimalConfigReturnsResults()
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
        IReadOnlyList<object> results = quotes.ExecuteFromJson(json);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<EmaResult>();
    }

    [TestMethod]
    public void ExecuteFromJsonWithInvalidJsonThrowsArgumentException()
    {
        // Arrange
        string json = "{ invalid json }";
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteFromJson(json);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Invalid JSON configuration*");
    }

    [TestMethod]
    public void ExecuteFromJsonWithNullJsonThrowsArgumentNullException()
    {
        // Arrange
        string json = null!;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteFromJson(json);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*json*");
    }

    [TestMethod]
    public void ExecuteFromJsonWithEmptyJsonThrowsArgumentException()
    {
        // Arrange
        string json = "";
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteFromJson(json);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*JSON configuration cannot be null or empty*");
    }

    [TestMethod]
    public void ExecuteFromJsonWithNullQuotesThrowsArgumentNullException()
    {
        // Arrange
        var config = new IndicatorConfig { Id = "RSI", Style = Style.Series };
        string json = JsonSerializer.Serialize(config);
        IEnumerable<IQuote> quotes = null!;

        // Act & Assert
        Action act = () => quotes.ExecuteFromJson(json);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*quotes*");
    }

    [TestMethod]
    public void ExecuteByIdWithSmaReturnsResults()
    {
        // Arrange
        string id = "SMA";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        var parameters = new Dictionary<string, object> { { "lookbackPeriods", 10 } };

        // Act
        IReadOnlyList<object> results = quotes.ExecuteById(id, style, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().AllBeOfType<SmaResult>();

        // Verify the results match what we'd get from calling Sma directly
        IReadOnlyList<SmaResult> directResults = quotes.ToSma(10);
        results.Should().HaveCount(directResults.Count);
    }

    [TestMethod]
    public void ExecuteFromJsonRoundTripProducesConsistentResults()
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
        IReadOnlyList<object> jsonResults = quotes.ExecuteFromJson(json);
        IReadOnlyList<object> directResults = quotes.ExecuteById(originalConfig.Id, originalConfig.Style, originalConfig.Parameters);

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