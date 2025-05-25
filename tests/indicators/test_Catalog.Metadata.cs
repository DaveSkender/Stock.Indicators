using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class MetadataTests
{
    [TestMethod]
    public void Metadata_ToJson_GeneratesValidJson()
    {
        // Arrange & Act
        string json = Metadata.ToJson();
        
        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        
        // Should deserialize without errors
        var deserializedArray = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.AreEqual(JsonValueKind.Array, deserializedArray.ValueKind);
    }
    
    [TestMethod]
    public void Metadata_ToJson_WithoutChartConfig_ExcludesChartConfig()
    {
        // Arrange & Act
        string json = Metadata.ToJson(includeChartConfig: false);
        
        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(json));
        
        // Should deserialize without errors
        var deserializedArray = JsonSerializer.Deserialize<JsonElement>(json);
        
        // Check the first element for chart-related properties
        var firstElement = deserializedArray.EnumerateArray().FirstOrDefault();
        
        // Verify chart style properties are excluded
        if (!firstElement.TryGetProperty("results", out var results))
        {
            Assert.Fail("Results property not found");
        }
        
        var firstResult = results.EnumerateArray().FirstOrDefault();
        Assert.IsFalse(firstResult.TryGetProperty("defaultColor", out _));
        Assert.IsFalse(firstResult.TryGetProperty("lineType", out _));
    }
    
    [TestMethod]
    public void Metadata_GetById_ReturnsCorrectIndicator()
    {
        // Arrange - Using SMA as a known indicator
        string indicatorId = "SMA";
        
        // Act
        var indicator = Metadata.GetById(indicatorId);
        
        // Assert
        Assert.IsNotNull(indicator);
        Assert.AreEqual(indicatorId, indicator.Uiid);
    }
    
    [TestMethod]
    public void Metadata_GetById_WithInvalidId_ReturnsNull()
    {
        // Arrange
        string invalidId = "NonExistentIndicator";
        
        // Act
        var indicator = Metadata.GetById(invalidId);
        
        // Assert
        Assert.IsNull(indicator);
    }
}
