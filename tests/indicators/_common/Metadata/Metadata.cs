using System.Text.Json;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Tests.Indicators;

[TestClass]
public class MetadataTests
{
    private static readonly Uri BaseUrl = new("https://example.com");
    private const string TestData = "_common/Metadata/listings.json";

    [TestMethod]
    public void IndicatorListing_ShouldReturnExpectedData()
    {
        // Arrange
        string json = File.ReadAllText(TestData);
        List<IndicatorListing> expectedListings = JsonSerializer.Deserialize<List<IndicatorListing>>(json);

        // Act
        List<IndicatorListing> result = Metadata.IndicatorListing(BaseUrl).ToList();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedListings.Count, result.Count);
        result.Should().BeEquivalentTo(expectedListings);
    }
}
