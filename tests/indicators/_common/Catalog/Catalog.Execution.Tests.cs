using System.Text.Json;

namespace Catalogging;

/// <summary>
/// Catalog execution tests validating dynamic invocation by ID and JSON config:
/// - ExecuteById for RSI/EMA/SMA (with/without parameters) matching direct calls
/// - JSON-based execution (valid, minimal, invalid inputs)
/// - Error handling for invalid id/style, null quotes, bad parameter types
/// - Defaults usage when required parameters are omitted
/// </summary>
[TestClass]
public class CatalogExecutionTests : TestBase
{
    [TestMethod]
    public void ExecuteByIdRsiDefault()
    {
        // Arrange
        const string id = "RSI";
        const Style style = Style.Series;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        // Act
        IReadOnlyList<RsiResult> results = quotes.ExecuteById<RsiResult>(id, style);

        // Assert
        results.Should().NotBeNullOrEmpty();

        IReadOnlyList<RsiResult> directResults = quotes.ToRsi();
        results.Should().HaveCount(directResults.Count);

        for (int i = 0; i < results.Count; i++)
        {
            results[i].Timestamp.Should().Be(directResults[i].Timestamp);
            results[i].Rsi.Should().Be(directResults[i].Rsi);
        }
    }

    [TestMethod]
    public void ExecuteByIdRsiWithParameters()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Dictionary<string, object> parameters = new() { { "lookbackPeriods", 10 } };

        // Act
        IReadOnlyList<RsiResult> results = quotes.ExecuteById<RsiResult>("RSI", Style.Series, parameters);

        // Assert
        results.Should().NotBeNullOrEmpty();
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi(10);
        results.Should().HaveCount(directResults.Count);
        for (int i = 0; i < results.Count; i++)
        {
            results[i].Timestamp.Should().Be(directResults[i].Timestamp);
            results[i].Rsi.Should().Be(directResults[i].Rsi);
        }
    }

    [TestMethod]
    public void ExecuteByIdEmaWithParameters()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Dictionary<string, object> parameters = new() { { "lookbackPeriods", 20 } };

        // Act
        IReadOnlyList<EmaResult> results = quotes.ExecuteById<EmaResult>("EMA", Style.Series, parameters);

        // Assert
        results.Should().NotBeNullOrEmpty();
        IReadOnlyList<EmaResult> directResults = quotes.ToEma(20);
        results.Should().HaveCount(directResults.Count);
        for (int i = 0; i < results.Count; i++)
        {
            results[i].Timestamp.Should().Be(directResults[i].Timestamp);
            results[i].Ema.Should().Be(directResults[i].Ema);
        }
    }

    [TestMethod]
    public void ExecuteByIdSmaWithParameters()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Dictionary<string, object> parameters = new() { { "lookbackPeriods", 10 } };

        // Act
        IReadOnlyList<SmaResult> results = quotes.ExecuteById<SmaResult>("SMA", Style.Series, parameters);

        // Assert
        results.Should().NotBeNullOrEmpty();
        IReadOnlyList<SmaResult> directResults = quotes.ToSma(10);
        results.Should().HaveCount(directResults.Count);
        for (int i = 0; i < results.Count; i++)
        {
            results[i].Timestamp.Should().Be(directResults[i].Timestamp);
            results[i].Sma.Should().Be(directResults[i].Sma);
        }
    }

    [TestMethod]
    public void ExecuteByIdInvalidInputs()
    {
        // invalid id
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Action a1 = () => quotes.ExecuteById<RsiResult>("INVALID_INDICATOR", Style.Series);
        a1.Should().Throw<InvalidOperationException>().WithMessage("*not found in catalog*");

        // null quotes
        IEnumerable<IQuote> nullQuotes = null!;
        Action a2 = () => nullQuotes.ExecuteById<RsiResult>("RSI", Style.Series);
        a2.Should().Throw<ArgumentNullException>().WithMessage("*quotes*");

        // empty id
        Action a3 = () => quotes.ExecuteById<RsiResult>(string.Empty, Style.Series);
        a3.Should().Throw<ArgumentException>().WithMessage("*ID cannot be null or empty*");

        // invalid style
        Action a4 = () => quotes.ExecuteById<object>("RSI", (Style)999);
        a4.Should().Throw<InvalidOperationException>().WithMessage("*not found in catalog*");

        // mismatched parameter type
        Dictionary<string, object> badParams = new() { { "lookbackPeriods", "invalid_string" } };
        Action a5 = () => quotes.ExecuteById<RsiResult>("RSI", Style.Series, badParams);
        a5.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void ExecuteByIdMissingRequiredParametersUsesDefaults()
    {
        // Arrange
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Dictionary<string, object> parameters = [];

        // Act
        IReadOnlyList<RsiResult> results = quotes.ExecuteById<RsiResult>("RSI", Style.Series, parameters);

        // Assert
        results.Should().NotBeNullOrEmpty();
        IReadOnlyList<RsiResult> defaultResults = quotes.ToRsi();
        results.Should().HaveCount(defaultResults.Count);
        for (int i = 0; i < results.Count; i++)
        {
            results[i].Timestamp.Should().Be(defaultResults[i].Timestamp);
            results[i].Rsi.Should().Be(defaultResults[i].Rsi);
        }
    }

    [TestMethod]
    public void ExecuteFromJsonRsiValid()
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
        results.Should().NotBeNullOrEmpty();
        IReadOnlyList<RsiResult> directResults = quotes.ToRsi(14);
        results.Should().HaveCount(directResults.Count);
        for (int i = 0; i < results.Count; i++)
        {
            results[i].Timestamp.Should().Be(directResults[i].Timestamp);
            results[i].Rsi.Should().Be(directResults[i].Rsi);
        }
    }

    [TestMethod]
    public void ExecuteFromJsonMinimalConfigWorks()
    {
        IndicatorConfig config = new() { Id = "EMA", Style = Style.Series };
        string json = JsonSerializer.Serialize(config);
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        IReadOnlyList<EmaResult> results = quotes.ExecuteFromJson<EmaResult>(json);
        results.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public void ExecuteFromJsonInvalidInputs()
    {
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();

        Action a1 = () => quotes.ExecuteFromJson<object>("{ invalid json }");
        a1.Should().Throw<ArgumentException>().WithMessage("*Invalid JSON configuration*");

        const string nullJson = null!;
        Action a2 = () => quotes.ExecuteFromJson<object>(nullJson);
        a2.Should().Throw<ArgumentNullException>().WithMessage("*json*");

        Action a3 = () => quotes.ExecuteFromJson<object>(string.Empty);
        a3.Should().Throw<ArgumentException>().WithMessage("*JSON configuration cannot be null or empty*");

        IEnumerable<IQuote> nullQuotes = null!;
        IndicatorConfig cfg = new() { Id = "RSI", Style = Style.Series };
        string json = JsonSerializer.Serialize(cfg);
        Action a4 = () => nullQuotes.ExecuteFromJson<object>(json);
        a4.Should().Throw<ArgumentNullException>().WithMessage("*quotes*");
    }

    [TestMethod]
    public void ExecuteFromJsonInvalidParameterTypesThrows()
    {
        const string json = """
        {
          "Id": "RSI",
          "Style": 0,
          "Parameters": {
            "lookbackPeriods": "not_a_number"
          }
        }
        """;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Action act = () => quotes.ExecuteFromJson<RsiResult>(json);
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void ExecuteFromJsonInvalidIdThrows()
    {
        const string json = """
        {
          "Id": "NONEXISTENT",
          "Style": 0
        }
        """;
        IReadOnlyList<Quote> quotes = Quotes.Take(50).ToList();
        Action act = () => quotes.ExecuteFromJson<RsiResult>(json);
        act.Should().Throw<InvalidOperationException>().WithMessage("*not found in catalog*");
    }
}
