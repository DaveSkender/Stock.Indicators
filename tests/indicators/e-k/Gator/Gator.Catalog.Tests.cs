namespace Catalogging;

/// <summary>
/// Test class for Gator catalog functionality.
/// </summary>
[TestClass]
public class GatorTests : TestBase
{
    [TestMethod]
    public void GatorSeriesListing()
    {
        // Act
        IndicatorListing listing = Gator.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Gator Oscillator");
        listing.Uiid.Should().Be("GATOR");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToGator");

        listing.Parameters?.Count.Should().Be(0);
        // No parameters for this indicator

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);

    }

    [TestMethod]
    public void GatorStreamListing()
    {
        // Act
        IndicatorListing listing = Gator.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Gator Oscillator");
        listing.Uiid.Should().Be("GATOR");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToGatorHub");

        listing.Parameters?.Count.Should().Be(0);
        // No parameters for this indicator

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);
    }
}
