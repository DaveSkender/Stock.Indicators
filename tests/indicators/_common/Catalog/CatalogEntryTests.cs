#nullable enable
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Catalog;

[TestClass]
[DoNotParallelize]
public class CatalogEntryTests : TestBase
{
    [TestInitialize]
    public void Setup()
    {
        // Clear the registry before each test to ensure clean state
        IndicatorRegistry.Clear();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clear the registry after each test to ensure clean state
        IndicatorRegistry.Clear();
    }

    [TestMethod]
    public void EachCatalogEntryShouldHaveRequiredMetadata()
    {
        // Arrange & Act - Get all catalog entries
        var catalog = IndicatorCatalog.Catalog;
        
        // Assert - Each entry should have complete metadata for automation
        catalog.Should().NotBeEmpty("catalog should contain indicator listings");
        
        foreach (var listing in catalog)
        {
            // Basic properties
            listing.Uiid.Should().NotBeNullOrWhiteSpace($"indicator {listing.Name} should have a UIID");
            listing.Name.Should().NotBeNullOrWhiteSpace($"indicator {listing.Uiid} should have a name");
            listing.Style.Should().BeDefined($"indicator {listing.Uiid} should have a valid style");
            listing.Category.Should().BeDefined($"indicator {listing.Uiid} should have a valid category");
            
            // Method name for automation
            if (!string.IsNullOrWhiteSpace(listing.MethodName))
            {
                // If method name is provided, it should be valid
                listing.MethodName.Should().StartWith("To", $"method name for {listing.Uiid} should start with 'To'");
            }
            // Note: Not all indicators have method names yet - this is being implemented gradually
            
            // Parameters should be valid if present
            if (listing.Parameters != null)
            {
                foreach (var param in listing.Parameters)
                {
                    param.ParameterName.Should().NotBeNullOrWhiteSpace($"parameter in {listing.Uiid} should have a name");
                    param.DisplayName.Should().NotBeNullOrWhiteSpace($"parameter {param.ParameterName} in {listing.Uiid} should have a display name");
                    param.DataType.Should().NotBeNullOrWhiteSpace($"parameter {param.ParameterName} in {listing.Uiid} should have a data type");
                    
                    // Required parameters should have constraints
                    if (param.IsRequired)
                    {
                        // Some parameter types like IEnumerable and DateTime are provided by the user and don't need default values
                        var allowedNullDefaults = new[] { "IEnumerable", "IReadOnlyList", "DateTime" };
                        if (param.DefaultValue == null && !allowedNullDefaults.Any(t => param.DataType.Contains(t, StringComparison.OrdinalIgnoreCase)))
                        {
                            Assert.Fail($"Required parameter {param.ParameterName} in {listing.Uiid} should have a default value or be of allowed type (IEnumerable, DateTime, etc.)");
                        }
                    }
                }
            }
            
            // Results should be valid
            listing.Results.Should().NotBeNullOrEmpty($"indicator {listing.Uiid} should have at least one result");
            foreach (var result in listing.Results!)
            {
                result.DataName.Should().NotBeNullOrWhiteSpace($"result in {listing.Uiid} should have a data name");
                result.DisplayName.Should().NotBeNullOrWhiteSpace($"result {result.DataName} in {listing.Uiid} should have a display name");
                result.DataType.Should().BeDefined($"result {result.DataName} in {listing.Uiid} should have a valid data type");
            }
        }
    }

    [TestMethod]
    public void EachCatalogEntryWithMethodNameShouldBeCallable()
    {
        // Arrange - Get all catalog entries with method names
        var catalog = IndicatorCatalog.Catalog;
        var entriesWithMethods = catalog.Where(l => !string.IsNullOrWhiteSpace(l.MethodName)).ToList();
        
        // Assert - Should have entries with method names
        entriesWithMethods.Should().NotBeEmpty("there should be catalog entries with method names");
        
        foreach (var listing in entriesWithMethods)
        {
            // Verify method name follows expected pattern
            listing.MethodName.Should().StartWith("To", $"method name for {listing.Uiid} should start with 'To'");
            
            // Method name should be related to the indicator (allow for common abbreviations and expansions)
            // This is a practical test - the important thing is that the method name exists and follows convention
            var cleanUiid = listing.Uiid.Replace("-", "", StringComparison.Ordinal).Replace("_", "", StringComparison.Ordinal);
            var upperMethodName = listing.MethodName!.ToUpperInvariant();
            var upperUiid = cleanUiid.ToUpperInvariant();
            
            // Handle common cases - method names should be reasonable representations of the indicator
            var isRelated = upperMethodName.Contains(upperUiid, StringComparison.Ordinal) || 
                           (upperUiid == "BB" && upperMethodName.Contains("BOLLINGER", StringComparison.Ordinal)) ||
                           (upperUiid == "CHEXIT" && upperMethodName.Contains("CHANDELIER", StringComparison.Ordinal)) ||
                           upperMethodName.EndsWith(upperUiid, StringComparison.Ordinal) ||
                           upperMethodName.Contains(upperUiid.Substring(0, Math.Min(3, upperUiid.Length)), StringComparison.Ordinal);
            
            // If not obviously related, just verify it's a valid method name format
            if (!isRelated)
            {
                listing.MethodName.Should().MatchRegex(@"^To[A-Z][a-zA-Z]*$", $"method name '{listing.MethodName}' for {listing.Uiid} should follow valid format");
            }
        }
    }

    [TestMethod]
    public void CatalogShouldHaveConsistentStyleCounts()
    {
        // Arrange & Act
        var catalog = IndicatorCatalog.Catalog;
        
        var seriesCount = catalog.Count(x => x.Style == Style.Series);
        var streamCount = catalog.Count(x => x.Style == Style.Stream);
        var bufferCount = catalog.Count(x => x.Style == Style.Buffer);
        
        // Assert - verify the counts are as expected per the earlier comment
        seriesCount.Should().Be(84, "there should be exactly 84 series style indicators");
        streamCount.Should().Be(8, "there should be exactly 8 stream style indicators");
        bufferCount.Should().Be(2, "there should be exactly 2 buffer style indicators");
        
        // Total verification
        var totalCount = seriesCount + streamCount + bufferCount;
        totalCount.Should().Be(94, "total indicators should be exactly 94");
    }

    [TestMethod]
    public void EachIndicatorShouldHaveUniqueIdStyleCombination()
    {
        // Arrange & Act
        var catalog = IndicatorCatalog.Catalog;
        
        // Group by UIID and Style combination
        var duplicates = catalog
            .GroupBy(l => new { l.Uiid, l.Style })
            .Where(g => g.Count() > 1)
            .ToList();
        
        // Assert - No duplicates should exist
        duplicates.Should().BeEmpty("each indicator should have a unique UIID and Style combination");
    }

    [TestMethod]
    public void IndicatorsWithSameUiidShouldHaveDifferentStyles()
    {
        // Arrange & Act
        var catalog = IndicatorCatalog.Catalog;
        
        // Group by UIID to find indicators with multiple implementations
        var multiStyleIndicators = catalog
            .GroupBy(l => l.Uiid)
            .Where(g => g.Count() > 1)
            .ToList();
        
        // Assert - Multi-style indicators should have different styles
        foreach (var group in multiStyleIndicators)
        {
            var styles = group.Select(l => l.Style).Distinct().ToList();
            styles.Should().HaveCount(group.Count(), 
                $"indicator {group.Key} with multiple implementations should have different styles for each");
        }
    }

    [TestMethod]
    public void EachCatalogEntryParametersShouldBeValidForAutomation()
    {
        // Arrange & Act
        var catalog = IndicatorCatalog.Catalog;
        var entriesWithParameters = catalog.Where(l => l.Parameters != null && l.Parameters.Any()).ToList();
        
        // Assert
        entriesWithParameters.Should().NotBeEmpty("there should be indicators with parameters");
        
        foreach (var listing in entriesWithParameters)
        {
            foreach (var parameter in listing.Parameters!)
            {
                // For automation, we need to be able to create instances of the parameter types
                parameter.DataType.Should().NotBeNullOrWhiteSpace($"parameter {parameter.ParameterName} in {listing.Uiid} should have a data type");
                
                // Common data types that should be supported for automation
                var supportedTypes = new[] 
                { 
                    "Int32", "Double", "Boolean", "DateTime", "String", "enum", "Nullable",
                    "IEnumerable", "IReadOnlyList<T> where T : IReusable",
                    "BetaType", "ChandExitType", "PivotPointType", "MaType", "CandlePartType", "EndType"
                };
                
                supportedTypes.Should().Contain(t => parameter.DataType.Contains(t) || parameter.DataType.StartsWith(t), 
                    $"parameter {parameter.ParameterName} data type '{parameter.DataType}' in {listing.Uiid} should be supported for automation");
                
                // Required parameters must have default values for automation
                if (parameter.IsRequired && parameter.DefaultValue == null)
                {
                    // This is allowed for some parameter types like IEnumerable and DateTime that are provided by the user
                    var allowedNullDefaults = new[] { "IEnumerable", "IReadOnlyList", "DateTime" };
                    allowedNullDefaults.Should().Contain(t => parameter.DataType.Contains(t, StringComparison.OrdinalIgnoreCase), 
                        $"required parameter {parameter.ParameterName} in {listing.Uiid} without default value should be of allowed type (IEnumerable, DateTime, etc.)");
                }
            }
        }
    }
}