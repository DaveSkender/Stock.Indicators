using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Catalog;

[TestClass]
[DoNotParallelize]
public class CatalogRegistryExtensions : TestBase
{
    [TestInitialize]
    public void Setup()
    {
        // Clear the registry before each test to ensure clean state
        IndicatorRegistry.Clear();
        // Re-register the catalog to populate the registry for testing
        IndicatorRegistry.RegisterCatalog();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clear the registry after each test to ensure clean state
        IndicatorRegistry.Clear();
    }

    [TestMethod]
    public void SearchByNameWithStyleShouldReturnCorrectIndicators()
    {
        // Act - Search for moving average indicators with Series style
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.SearchByName("Moving", Style.Series);

        // Assert - Should find EMA, SMA, and other moving average series indicators
        results.Should().NotBeEmpty("there should be moving average indicators with Series style");
        results.Should().Contain(l => l.Uiid == "EMA" && l.Style == Style.Series, "EMA Series should be found");
        results.Should().Contain(l => l.Uiid == "SMA" && l.Style == Style.Series, "SMA Series should be found");
        results.Should().NotContain(l => l.Style != Style.Series, "only Series style indicators should be returned");

        // All results should contain "Moving" in the name
        results.Should().OnlyContain(l => l.Name.Contains("Moving"), "all results should contain 'Moving' in the name");
    }

    [TestMethod]
    public void SearchByNameWithCategoryShouldReturnCorrectIndicators()
    {
        // Act - Search for moving average indicators in MovingAverage category
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.SearchByName("Moving", Category.MovingAverage);

        // Assert - Should find moving average indicators in the correct category
        results.Should().NotBeEmpty("there should be moving average indicators in MovingAverage category");
        results.Should().Contain(l => l.Uiid == "EMA", "EMA should be found");
        results.Should().Contain(l => l.Uiid == "SMA", "SMA should be found");
        results.Should().OnlyContain(l => l.Category == Category.MovingAverage, "only MovingAverage category indicators should be returned");
        results.Should().OnlyContain(l => l.Name.Contains("Moving"), "all results should contain 'Moving' in the name");
    }

    [TestMethod]
    public void GetIndicatorsWithResultTypeShouldReturnCorrectIndicators()
    {
        // Act - Get indicators with Default result type
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.GetIndicatorsWithResultType(ResultType.Default);

        // Assert - Should find indicators that have Default result type fields
        results.Should().NotBeEmpty("there should be indicators with Default result type");

        // All results should have at least one result with Default type
        results.Should().OnlyContain(l => l.Results.Any(r => r.DataType == ResultType.Default),
            "all results should have at least one result with Default type");
    }

    [TestMethod]
    public void GetIndicatorsWithResultNameShouldReturnCorrectIndicators()
    {
        // Act - Get indicators with results containing "Value" in the name
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.GetIndicatorsWithResultName("Value");

        // Assert - Should find indicators with "Value" in result names
        results.Should().NotBeEmpty("there should be indicators with 'Value' in result names");

        // All results should have at least one result containing "Value"
        results.Should().OnlyContain(l => l.Results != null && l.Results.Any(r => r.DataName.Contains("Value", StringComparison.OrdinalIgnoreCase) || r.DisplayName.Contains("Value", StringComparison.OrdinalIgnoreCase)),
            "all results should have at least one result containing 'Value'");
    }

    [TestMethod]
    public void GetIndicatorsSortedByNameShouldReturnSortedIndicators()
    {
        // Act - Get indicators sorted by name (ascending)
        IReadOnlyCollection<IndicatorListing> ascendingResults
            = IndicatorRegistryExtensions.GetIndicatorsSortedByName();

        // Assert - Ascending order
        ascendingResults.Should().NotBeEmpty("there should be indicators in the catalog");
        List<IndicatorListing> ascendingList = ascendingResults.ToList();

        // Use the same sorting logic as the implementation for comparison
        List<IndicatorListing> expectedAscending = ascendingList.OrderBy(i => i.Name).ToList();
        ascendingList.Should().BeEquivalentTo(expectedAscending, options => options.WithStrictOrdering(),
            "ascending results should match expected sort order");

        // Act - Get indicators sorted by name (descending)
        IReadOnlyCollection<IndicatorListing> descendingResults
            = IndicatorRegistryExtensions.GetIndicatorsSortedByName(ascending: false);

        // Assert - Descending order
        descendingResults.Should().HaveCount(ascendingResults.Count, "both sorts should return the same number of indicators");
        List<IndicatorListing> descendingList = descendingResults.ToList();

        // Use the same sorting logic as the implementation for comparison
        List<IndicatorListing> expectedDescending = ascendingList.OrderByDescending(i => i.Name).ToList();
        descendingList.Should().BeEquivalentTo(expectedDescending, options => options.WithStrictOrdering(),
            "descending results should match expected sort order");
    }

    [TestMethod]
    public void GetIndicatorsWithRequiredParametersShouldReturnCorrectIndicators()
    {
        // Act
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.GetIndicatorsWithRequiredParameters();

        // Assert - Should find indicators that have at least one required parameter
        results.Should().NotBeEmpty("there should be indicators with required parameters");

        // All results should have at least one required parameter
        results.Should().OnlyContain(l => l.Parameters != null && l.Parameters.Any(p => p.IsRequired),
            "all results should have at least one required parameter");

        // Verify specific indicators that we know have required parameters
        results.Should().Contain(l => l.Uiid == "EMA", "EMA should have required parameters");
        results.Should().Contain(l => l.Uiid == "SMA", "SMA should have required parameters");
    }

    [TestMethod]
    public void GetIndicatorsWithOptionalParametersShouldReturnCorrectIndicators()
    {
        // Act
        IReadOnlyCollection<IndicatorListing> results
            = IndicatorRegistryExtensions.GetIndicatorsWithOptionalParameters();

        // Assert - Should find indicators that have at least one optional (non-required) parameter
        results.Should().NotBeEmpty("there should be indicators with optional parameters");

        // All results should have at least one non-required parameter
        results.Should().OnlyContain(l => l.Parameters != null && l.Parameters.Any(p => !p.IsRequired),
            "all results should have at least one optional parameter");
    }
}
