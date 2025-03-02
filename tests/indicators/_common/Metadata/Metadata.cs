using System.Text.Json;

namespace Tests.Indicators;

[TestClass]
public class MetadataTests
{
    private const string BaseUrl = "https://example.com";
    private const string ListingsFilePath = "tests/indicators/_common/Metadata/listings.json";

    [TestMethod]
    public void IndicatorListing_ShouldReturnExpectedData()
    {
        // Arrange
        string json = File.ReadAllText(ListingsFilePath);
        List<IndicatorListing> expectedListings = JsonSerializer.Deserialize<List<IndicatorListing>>(json);

        // Act
        List<IndicatorListing> result = Metadata.IndicatorListing(BaseUrl).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedListings.Count, result.Count);
        result.Should().BeEquivalentTo(expectedListings);
    }
}
