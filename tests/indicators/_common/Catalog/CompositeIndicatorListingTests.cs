using FluentAssertions;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

/// <summary>
/// Tests for CompositeIndicatorListing functionality.
/// NOTE: CompositeIndicatorListing is obsolete. These tests are maintained to ensure
/// backward compatibility while the obsolete classes still exist.
/// For new multi-style indicators, use the one-listing-per-style approach.
/// See OneListingPerStyleGuide.md for guidance.
/// </summary>
[TestClass]
public class CompositeIndicatorListingTests
{
    [TestMethod]
    public void ShouldSupportMultipleStyles()
    {
        // Arrange
        var listing = new CompositeIndicatorListingBuilder()
            .WithName("Test Indicator")
            .WithId("TEST")
            .WithStyle(Style.Series)
            .WithSupportedStyles(Style.Series, Style.Stream, Style.Buffer)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        // Assert
        var compositeListing = listing as CompositeIndicatorListing;
        compositeListing.Should().NotBeNull();
        compositeListing!.SupportedStyles.Should().HaveCount(3);
        compositeListing.SupportedStyles.Should().Contain(Style.Series);
        compositeListing.SupportedStyles.Should().Contain(Style.Stream);
        compositeListing.SupportedStyles.Should().Contain(Style.Buffer);
        compositeListing.Style.Should().Be(Style.Series); // Primary style
    }

    [TestMethod]
    public void ShouldFindCompositeIndicatorByAllSupportedStyles()
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
    public void WithNoStylesShouldUseDefaultStyle()
    {
        // Arrange & Act
        var listing = new CompositeIndicatorListingBuilder()
            .WithName("Default Style Indicator")
            .WithId("DEFAULT")
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        // Assert
        var compositeListing = listing as CompositeIndicatorListing;
        compositeListing.Should().NotBeNull();
        compositeListing!.SupportedStyles.Should().HaveCount(1);
        compositeListing.SupportedStyles.Should().Contain(Style.Series);
    }

    [TestMethod]
    public void AddSupportedStyleShouldAddStylesWithoutDuplicates()
    {
        // Arrange
        var builder = new CompositeIndicatorListingBuilder()
            .WithName("Test Indicator")
            .WithId("TEST")
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true);

        // Act
        builder.AddSupportedStyle(Style.Series); // Already added via WithStyle
        builder.AddSupportedStyle(Style.Stream);
        builder.AddSupportedStyle(Style.Stream); // Duplicate
        builder.AddSupportedStyle(Style.Buffer);

        var listing = builder.Build();

        // Assert
        listing.Should().BeOfType<CompositeIndicatorListing>();
        var compositeListing = (CompositeIndicatorListing)listing;
        compositeListing.SupportedStyles.Should().HaveCount(3);
        compositeListing.SupportedStyles.Should().Contain(Style.Series);
        compositeListing.SupportedStyles.Should().Contain(Style.Stream);
        compositeListing.SupportedStyles.Should().Contain(Style.Buffer);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Reset the registry after each test
        IndicatorRegistry.Clear();
    }
}
