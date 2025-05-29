using FluentAssertions;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

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
        // Arrange
        IndicatorRegistry.Clear();
        var compositeListing = new CompositeIndicatorListingBuilder()
            .WithName("Multi-Style Indicator")
            .WithId("MULTI")
            .WithStyle(Style.Series)
            .WithSupportedStyles(Style.Series, Style.Stream, Style.Buffer)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        var seriesOnlyListing = new IndicatorListingBuilder()
            .WithName("Series-Only Indicator")
            .WithId("SERIES")
            .WithStyle(Style.Series)
            .WithCategory(Category.Undefined)
            .AddResult("Value", "Value", ResultType.Default, isDefault: true)
            .Build();

        IndicatorRegistry.Register(compositeListing);
        IndicatorRegistry.Register(seriesOnlyListing);

        // Act & Assert
        var seriesCatalog = IndicatorRegistry.GetCatalog(Style.Series);
        var streamCatalog = IndicatorRegistry.GetCatalog(Style.Stream);
        var bufferCatalog = IndicatorRegistry.GetCatalog(Style.Buffer);

        // Multi-style indicator should appear in all catalogs
        seriesCatalog.Should().Contain(i => i.Uiid == "MULTI");
        streamCatalog.Should().Contain(i => i.Uiid == "MULTI");
        bufferCatalog.Should().Contain(i => i.Uiid == "MULTI");

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
