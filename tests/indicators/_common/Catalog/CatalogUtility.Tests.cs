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
        IReadOnlyList<RsiResult> results = quotes.ExecuteById<RsiResult>(id, style);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.Should().HaveCountGreaterThan(0);

        // Check that we got actual RSI results
        // typed results

        // Verify the results match what we'd get from calling Rsi directly
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi();
        results.Should().HaveCount(directResults.Count);

        // Compare actual values
        for (int i = 0; i < results.Count; i++)
        {
            RsiResult result = results[i];
            RsiResult directResult = directResults[i];

            result.Timestamp.Should().Be(directResult.Timestamp);
            result.Rsi.Should().Be(directResult.Rsi);
        }
    }

    [TestMethod]
    public void ExecuteByIdWithParametersReturnsResults()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Dictionary<string, object> parameters = new() { { "lookbackPeriods", 10 } };

        // Act
        IReadOnlyList<RsiResult> results = quotes.ExecuteById<RsiResult>(id, style, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        // typed results

        // Verify the results match what we'd get from calling Rsi directly with same parameters
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi(10);
        results.Should().HaveCount(directResults.Count);

        // Compare actual values
        for (int i = 0; i < results.Count; i++)
        {
            RsiResult result = results[i];
            RsiResult directResult = directResults[i];

            result.Timestamp.Should().Be(directResult.Timestamp);
            result.Rsi.Should().Be(directResult.Rsi);
        }
    }

    [TestMethod]
    public void ExecuteByIdWithEmaReturnsResults()
    {
        // Arrange
        string id = "EMA";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Dictionary<string, object> parameters = new() { { "lookbackPeriods", 20 } };

        // Act
        IReadOnlyList<EmaResult> results = quotes.ExecuteById<EmaResult>(id, style, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        // typed results

        // Verify the results match what we'd get from calling Ema directly
        IReadOnlyList<EmaResult> directResults = quotes.ToEma(20);
        results.Should().HaveCount(directResults.Count);

        // Compare actual values
        for (int i = 0; i < results.Count; i++)
        {
            EmaResult result = results[i];
            EmaResult directResult = directResults[i];

            result.Timestamp.Should().Be(directResult.Timestamp);
            result.Ema.Should().Be(directResult.Ema);
        }
    }

    [TestMethod]
    public void ExecuteByIdWithInvalidIdThrowsException()
    {
        // Arrange
        string id = "INVALID_INDICATOR";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteById<RsiResult>(id, style);
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
        Action act = () => quotes.ExecuteById<RsiResult>(id, style);
        act.Should().Throw<ArgumentNullException>()
           .WithMessage("*quotes*");
    }

    [TestMethod]
    public void ExecuteByIdWithEmptyIdThrowsArgumentException()
    {
        // Arrange
        string id = "";
        Style style = Style.Series;
        IEnumerable<Quote> quotes = Quotes.Take(50);

        // Act & Assert
        Action act = () => quotes.ExecuteById<RsiResult>(id, style);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*ID cannot be null or empty*");
    }

    [TestMethod]
    public void ExecuteFromJsonWithValidConfigReturnsResults()
    {
        // Arrange
        IndicatorConfig config = new() {
            Id = "RSI",
            Style = Style.Series,
            Parameters = new Dictionary<string, object> { { "lookbackPeriods", 14 } }
        };
        string json = JsonSerializer.Serialize(config);
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<RsiResult> results = quotes.ExecuteFromJson<RsiResult>(json);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        // typed results

        // Verify the results match what we'd get from calling Rsi directly
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi(14);
        results.Should().HaveCount(directResults.Count);

        // Compare actual values
        for (int i = 0; i < results.Count; i++)
        {
            RsiResult result = results[i];
            RsiResult directResult = directResults[i];

            result.Timestamp.Should().Be(directResult.Timestamp);
            result.Rsi.Should().Be(directResult.Rsi);
        }
    }

    [TestMethod]
    public void ExecuteFromJsonWithMinimalConfigReturnsResults()
    {
        // Arrange - minimal configuration with just ID and Style
        IndicatorConfig config = new() {
            Id = "EMA",
            Style = Style.Series
        };
        string json = JsonSerializer.Serialize(config);
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<EmaResult> results = quotes.ExecuteFromJson<EmaResult>(json);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        // typed results
    }

    [TestMethod]
    public void ExecuteFromJsonWithInvalidJsonThrowsArgumentException()
    {
        // Arrange
        string json = "{ invalid json }";
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteFromJson<object>(json);
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
        Action act = () => quotes.ExecuteFromJson<object>(json);
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
        Action act = () => quotes.ExecuteFromJson<object>(json);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*JSON configuration cannot be null or empty*");
    }

    [TestMethod]
    public void ExecuteFromJsonWithNullQuotesThrowsArgumentNullException()
    {
        // Arrange
        IndicatorConfig config = new() { Id = "RSI", Style = Style.Series };
        string json = JsonSerializer.Serialize(config);
        IEnumerable<IQuote> quotes = null!;

        // Act & Assert
        Action act = () => quotes.ExecuteFromJson<object>(json);
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
        Dictionary<string, object> parameters = new() { { "lookbackPeriods", 10 } };

        // Act
        IReadOnlyList<SmaResult> results = quotes.ExecuteById<SmaResult>(id, style, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        // typed results

        // Verify the results match what we'd get from calling Sma directly
        IReadOnlyList<SmaResult> directResults = quotes.ToSma(10);
        results.Should().HaveCount(directResults.Count);

        // Compare actual values
        for (int i = 0; i < results.Count; i++)
        {
            SmaResult result = results[i];
            SmaResult directResult = directResults[i];

            result.Timestamp.Should().Be(directResult.Timestamp);
            result.Sma.Should().Be(directResult.Sma);
        }
    }

    [TestMethod]
    public void ExecuteFromJsonRoundTripProducesConsistentResults()
    {
        // Arrange - test that we can serialize and deserialize a config and get the same results
        IndicatorConfig originalConfig = new() {
            Id = "EMA",
            Style = Style.Series,
            Parameters = new Dictionary<string, object> { { "lookbackPeriods", 20 } },
            DisplayName = "Test EMA",
            Description = "Test description"
        };

        string json = JsonSerializer.Serialize(originalConfig);
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<EmaResult> jsonResults = quotes.ExecuteFromJson<EmaResult>(json);
        IReadOnlyList<EmaResult> directResults = quotes.ExecuteById<EmaResult>(originalConfig.Id, originalConfig.Style, originalConfig.Parameters);

        // Assert
        jsonResults.Should().NotBeNull();
        directResults.Should().NotBeNull();
        jsonResults.Should().HaveCount(directResults.Count);
        // typed results

        // Compare actual values
        for (int i = 0; i < jsonResults.Count; i++)
        {
            EmaResult jsonResult = jsonResults[i];
            EmaResult directResult = directResults[i];

            jsonResult.Timestamp.Should().Be(directResult.Timestamp);
            jsonResult.Ema.Should().Be(directResult.Ema);
        }
    }

    [TestMethod]
    public void ExecuteByIdWithInvalidStyleThrowsException()
    {
        // Arrange
        string id = "RSI";
        Style style = (Style)999; // Invalid style value
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteById<object>(id, style);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not found in registry*");
    }

    [TestMethod]
    public void ExecuteByIdWithMismatchedParameterTypeThrowsException()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Dictionary<string, object> parameters = new() { { "lookbackPeriods", "invalid_string" } };

        // Act & Assert
        Action act = () => quotes.ExecuteById<RsiResult>(id, style, parameters);
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void ExecuteByIdWithMissingRequiredParametersUsesDefaults()
    {
        // Arrange
        string id = "RSI";
        Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Dictionary<string, object> parameters = [];

        // Act
        IReadOnlyList<RsiResult> results = quotes.ExecuteById<RsiResult>(id, style, parameters);

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        // typed results

        // Should match default RSI behavior
        IReadOnlyList<RsiResult> defaultResults = quotes.ToRsi();
        results.Should().HaveCount(defaultResults.Count);

        // Compare actual values to ensure defaults were used correctly
        for (int i = 0; i < results.Count; i++)
        {
            RsiResult result = results[i];
            RsiResult defaultResult = defaultResults[i];

            result.Timestamp.Should().Be(defaultResult.Timestamp);
            result.Rsi.Should().Be(defaultResult.Rsi);
        }
    }

    [TestMethod]
    public void ExecuteFromJsonWithInvalidParameterTypesThrowsException()
    {
        // Arrange - JSON that deserializes to wrong parameter types
        string json = """
        {
          "Id": "RSI",
          "Style": 0,
          "Parameters": {
            "lookbackPeriods": "not_a_number"
          }
        }
        """;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteFromJson<RsiResult>(json);
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void ExecuteFromJsonWithInvalidIdThrowsException()
    {
        // Arrange
        string json = """
        {
          "Id": "NONEXISTENT",
          "Style": 0
        }
        """;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act & Assert
        Action act = () => quotes.ExecuteFromJson<RsiResult>(json);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not found in registry*");
    }
}
