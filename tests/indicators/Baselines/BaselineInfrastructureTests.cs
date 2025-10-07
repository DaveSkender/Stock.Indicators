using System.Text.Json;
using Tests.Indicators.Baselines.Models;

namespace Tests.Indicators.Baselines;

[TestClass]
public class BaselineInfrastructureTests : TestBase
{
    [TestMethod]
    public void RoundTripSerialization()
    {
        // Arrange: Create a baseline file with sample data
        BaselineMetadata metadata = new(
            IndicatorName: "Sma",
            ScenarioName: "Standard",
            GeneratedAt: new DateTime(2025, 10, 6, 12, 34, 56, DateTimeKind.Utc),
            LibraryVersion: "3.0.0",
            WarmupPeriodCount: 19
        );

        Dictionary<string, double?> properties1 = new() {
            { "sma", null }
        };

        Dictionary<string, double?> properties2 = new() {
            { "sma", 214.52331349206352 }
        };

        List<BaselineResult> results = [
            new BaselineResult(new DateTime(2016, 1, 4), properties1),
            new BaselineResult(new DateTime(2016, 1, 5), properties1),
            new BaselineResult(new DateTime(2016, 2, 1), properties2)
        ];

        BaselineFile original = new(metadata, results);

        // Act: Serialize and deserialize
        string json = BaselineWriter.ToJson(original);
        BaselineFile deserialized = BaselineReader.FromJson(json);

        // Assert: Verify round-trip
        deserialized.Should().NotBeNull();
        deserialized.Metadata.IndicatorName.Should().Be("Sma");
        deserialized.Metadata.ScenarioName.Should().Be("Standard");
        deserialized.Metadata.LibraryVersion.Should().Be("3.0.0");
        deserialized.Metadata.WarmupPeriodCount.Should().Be(19);
        deserialized.Results.Should().HaveCount(3);

        // Verify first result (null value)
        deserialized.Results[0].Date.Should().Be(new DateTime(2016, 1, 4));
        deserialized.Results[0].Properties["sma"].Should().BeNull();

        // Verify last result (numeric value)
        deserialized.Results[2].Date.Should().Be(new DateTime(2016, 2, 1));
        deserialized.Results[2].Properties["sma"].Should().Be(214.52331349206352);
    }

    [TestMethod]
    public void SerializationUsesExpectedFormat()
    {
        // Arrange
        BaselineMetadata metadata = new(
            IndicatorName: "Sma",
            ScenarioName: "Standard",
            GeneratedAt: new DateTime(2025, 10, 6, 12, 34, 56, DateTimeKind.Utc),
            LibraryVersion: "3.0.0",
            WarmupPeriodCount: 19
        );

        Dictionary<string, double?> properties = new() {
            { "sma", 214.52 }
        };

        List<BaselineResult> results = [
            new BaselineResult(new DateTime(2016, 1, 4), properties)
        ];

        BaselineFile baselineFile = new(metadata, results);

        // Act
        string json = BaselineWriter.ToJson(baselineFile);

        // Assert: Verify JSON format expectations
        json.Should().Contain("\"metadata\"");
        json.Should().Contain("\"results\"");
        json.Should().Contain("\"indicatorName\": \"Sma\"");
        json.Should().Contain("\"scenarioName\": \"Standard\"");
        json.Should().Contain("\"date\": \"2016-01-04\"");
        json.Should().Contain("\"sma\": 214.52");
    }

    [TestMethod]
    public void WriteAndReadFromFile()
    {
        // Arrange
        string tempFile = Path.Combine(Path.GetTempPath(), $"test-baseline-{Guid.NewGuid()}.json");

        try
        {
            BaselineMetadata metadata = new(
                IndicatorName: "Macd",
                ScenarioName: "Standard",
                GeneratedAt: DateTime.UtcNow,
                LibraryVersion: "3.0.0",
                WarmupPeriodCount: 33
            );

            Dictionary<string, double?> properties = new() {
                { "macd", 1.2345 },
                { "signal", 0.9876 },
                { "histogram", 0.2469 }
            };

            List<BaselineResult> results = [
                new BaselineResult(new DateTime(2016, 2, 15), properties)
            ];

            BaselineFile original = new(metadata, results);

            // Act: Write to file
            BaselineWriter.WriteToFile(original, tempFile);

            // Assert: File exists
            File.Exists(tempFile).Should().BeTrue();

            // Act: Read from file
#nullable enable
            BaselineFile? loaded = BaselineReader.ReadFromFile(tempFile);
#nullable disable

            // Assert: Content matches
            loaded.Should().NotBeNull();
            loaded!.Metadata.IndicatorName.Should().Be("Macd");
            loaded.Results.Should().HaveCount(1);
            loaded.Results[0].Properties["macd"].Should().Be(1.2345);
            loaded.Results[0].Properties["signal"].Should().Be(0.9876);
            loaded.Results[0].Properties["histogram"].Should().Be(0.2469);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [TestMethod]
    public void ReadFromFileMissingFile()
    {
        // Arrange
        string nonExistentFile = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid()}.json");

        // Act
#nullable enable
        BaselineFile? result = BaselineReader.ReadFromFile(nonExistentFile);
#nullable disable

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public void FromJsonMalformedJson()
    {
        // Arrange
        string malformedJson = "{ invalid json }";

        // Act & Assert
        Action act = () => BaselineReader.FromJson(malformedJson);
        act.Should().Throw<JsonException>()
            .WithMessage("Malformed baseline JSON*");
    }

    [TestMethod]
    public void FromJsonMissingRequiredFields()
    {
        // Arrange: JSON missing indicatorName
        string incompleteJson = """
            {
                "metadata": {
                    "scenarioName": "Standard",
                    "generatedAt": "2025-10-06T12:34:56Z",
                    "libraryVersion": "3.0.0",
                    "warmupPeriodCount": 19
                },
                "results": [
                    { "date": "2016-01-04", "sma": null }
                ]
            }
            """;

        // Act & Assert
        Action act = () => BaselineReader.FromJson(incompleteJson);
        act.Should().Throw<JsonException>()
            .WithMessage("*IndicatorName*");
    }

    [TestMethod]
    public void SerializesNullValuesExplicitly()
    {
        // Arrange
        BaselineMetadata metadata = new(
            IndicatorName: "Test",
            ScenarioName: "Standard",
            GeneratedAt: DateTime.UtcNow,
            LibraryVersion: "3.0.0",
            WarmupPeriodCount: 1
        );

        Dictionary<string, double?> properties = new() {
            { "value", null }
        };

        List<BaselineResult> results = [
            new BaselineResult(new DateTime(2016, 1, 4), properties)
        ];

        BaselineFile baselineFile = new(metadata, results);

        // Act
        string json = BaselineWriter.ToJson(baselineFile);

        // Assert: Verify null is explicitly present
        json.Should().Contain("\"value\": null");
    }
}
