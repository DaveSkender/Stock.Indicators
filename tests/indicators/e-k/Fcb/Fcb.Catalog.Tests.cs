namespace Catalogging;

/// <summary>
/// Test class for Fcb catalog functionality.
/// </summary>
[TestClass]
public class FcbTests : TestBase
{
    [TestMethod]
    public void FcbSeriesListing()
    {
        // Act
        IndicatorListing listing = Fcb.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Fractal Chaos Bands");
        listing.Uiid.Should().Be("FCB");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceChannel);
        listing.MethodName.Should().Be("ToFcb");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam windowSpanParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "windowSpan");
        windowSpanParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        IndicatorResult upperbandResult = listing.Results.SingleOrDefault(r => r.DataName == "UpperBand");
        upperbandResult.Should().NotBeNull();
        upperbandResult?.DisplayName.Should().Be("Upper Band");
        upperbandResult.IsReusable.Should().Be(true);
        IndicatorResult lowerbandResult1 = listing.Results.SingleOrDefault(r => r.DataName == "LowerBand");
        lowerbandResult1.Should().NotBeNull();
        lowerbandResult1?.DisplayName.Should().Be("Lower Band");
        lowerbandResult1.IsReusable.Should().Be(false);
    }
}
