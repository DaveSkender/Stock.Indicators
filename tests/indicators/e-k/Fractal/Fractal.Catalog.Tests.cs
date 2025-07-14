namespace Catalog;

[TestClass]
public class FractalTests : TestBase
{
    [TestMethod]
    public void FractalSeriesListing()
    {
        // Act
        var listing = Fractal.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Williams Fractal (high/low)");
        listing.Uiid.Should().Be("FRACTAL");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PricePattern);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        var windowSpan = listing.Parameters.FirstOrDefault(p => p.ParameterName == "windowSpan");
        windowSpan.Should().NotBeNull();
        windowSpan!.DisplayName.Should().Be("Window Span");
        windowSpan.Description.Should().Be("Number of periods to look back and forward for the calculation");
        windowSpan.IsRequired.Should().BeFalse();

        var endType = listing.Parameters.FirstOrDefault(p => p.ParameterName == "endType");
        endType.Should().NotBeNull();
        endType!.DisplayName.Should().Be("End Type");
        endType.Description.Should().Be("Type of price to use for the calculation");
        endType.IsRequired.Should().BeFalse();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        var bearResult = listing.Results[0];
        bearResult.DataName.Should().Be("FractalBear");
        bearResult.DisplayName.Should().Be("Bear Fractal");
        bearResult.DataType.Should().Be(ResultType.Default);
        bearResult.IsReusable.Should().BeFalse();

        var bullResult = listing.Results[1];
        bullResult.DataName.Should().Be("FractalBull");
        bullResult.DisplayName.Should().Be("Bull Fractal");
        bullResult.DataType.Should().Be(ResultType.Default);
        bullResult.IsReusable.Should().BeTrue();
    }
}
