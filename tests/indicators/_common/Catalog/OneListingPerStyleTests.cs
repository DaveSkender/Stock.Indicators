using FluentAssertions;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

/// <summary>
/// Tests for the one-listing-per-style approach for multi-style indicators.
/// This approach creates separate indicator listings for each style (Series, Stream, Buffer)
/// instead of using composite listings.
/// See OneListingPerStyleGuide.md for guidance.
/// </summary>
[TestClass]
public class OneListingPerStyleTests
{
    [TestMethod]
    public void ShouldCreateSeparateListingsForEachStyle()
    {
        // Arrange: Create style-specific listings using the one-listing-per-style approach
        IndicatorRegistry.Clear();

        var seriesListing = new IndicatorListingBuilder()
            .WithName("Multi Style Test")
            .WithId("MULTI-Series")
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        var streamListing = new IndicatorListingBuilder()
            .WithName("Multi Style Test")
            .WithId("MULTI-Stream")
            .WithStyle(Style.Stream)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        var bufferListing = new IndicatorListingBuilder()
            .WithName("Multi Style Test")
            .WithId("MULTI-Buffer")
            .WithStyle(Style.Buffer)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        // Assert: Each listing should have the correct style
        seriesListing.Style.Should().Be(Style.Series);
        seriesListing.Uiid.Should().Be("MULTI-Series");

        streamListing.Style.Should().Be(Style.Stream);
        streamListing.Uiid.Should().Be("MULTI-Stream");

        bufferListing.Style.Should().Be(Style.Buffer);
        bufferListing.Uiid.Should().Be("MULTI-Buffer");
    }

    [TestMethod]
    public void ShouldFindMultiStyleIndicatorByAllSupportedStyles()
    {
        // Arrange: Create style-specific listings using the one-listing-per-style approach
        IndicatorRegistry.Clear();

        var seriesListing = new IndicatorListingBuilder()
            .WithName("Multi Style Test")
            .WithId("MULTI-Series")
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        var streamListing = new IndicatorListingBuilder()
            .WithName("Multi Style Test")
            .WithId("MULTI-Stream")
            .WithStyle(Style.Stream)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        var bufferListing = new IndicatorListingBuilder()
            .WithName("Multi Style Test")
            .WithId("MULTI-Buffer")
            .WithStyle(Style.Buffer)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        var seriesOnlyListing = new IndicatorListingBuilder()
            .WithName("Series Only Test")
            .WithId("SERIES")
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        // Register all listings
        IndicatorRegistry.Register(seriesListing);
        IndicatorRegistry.Register(streamListing);
        IndicatorRegistry.Register(bufferListing);
        IndicatorRegistry.Register(seriesOnlyListing);

        // Act & Assert
        var seriesCatalog = IndicatorRegistry.GetCatalog(Style.Series);
        var streamCatalog = IndicatorRegistry.GetCatalog(Style.Stream);
        var bufferCatalog = IndicatorRegistry.GetCatalog(Style.Buffer);

        // Multi-style indicator should appear in all catalogs with style-specific IDs
        seriesCatalog.Should().Contain(i => i.Uiid == "MULTI-Series");
        streamCatalog.Should().Contain(i => i.Uiid == "MULTI-Stream");
        bufferCatalog.Should().Contain(i => i.Uiid == "MULTI-Buffer");

        // Series-only indicator should only appear in series catalog
        seriesCatalog.Should().Contain(i => i.Uiid == "SERIES");
        streamCatalog.Should().NotContain(i => i.Uiid == "SERIES");
        bufferCatalog.Should().NotContain(i => i.Uiid == "SERIES");
    }

    [TestMethod]
    public void ShouldCreateConsistentListingsAcrossStyles()
    {
        // Arrange: Create listings for the same indicator across different styles
        var baseName = "Consistent Test Indicator";
        var baseCategory = Category.Oscillator;

        var seriesListing = new IndicatorListingBuilder()
            .WithName(baseName)
            .WithId("CONSISTENT-Series")
            .WithStyle(Style.Series)
            .WithCategory(baseCategory)
            .AddParameter<int>("Period", "period", 14, 1, 200, "Number of periods")
            .AddResult("Value", "Value", ResultType.Centerline, isDefault: true)
            .Build();

        var streamListing = new IndicatorListingBuilder()
            .WithName(baseName)
            .WithId("CONSISTENT-Stream")
            .WithStyle(Style.Stream)
            .WithCategory(baseCategory)
            .AddParameter<int>("Period", "period", 14, 1, 200, "Number of periods")
            .AddResult("Value", "Value", ResultType.Centerline, isDefault: true)
            .Build();

        // Assert: Both listings should have consistent metadata except for style and ID
        seriesListing.Name.Should().Be(streamListing.Name);
        seriesListing.Category.Should().Be(streamListing.Category);
        seriesListing.Parameters.Should().HaveCount(streamListing.Parameters.Count);
        seriesListing.Results.Should().HaveCount(streamListing.Results.Count);

        // But different styles and IDs
        seriesListing.Style.Should().NotBe(streamListing.Style);
        seriesListing.Uiid.Should().NotBe(streamListing.Uiid);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Reset the registry after each test
        IndicatorRegistry.Clear();
    }
}
