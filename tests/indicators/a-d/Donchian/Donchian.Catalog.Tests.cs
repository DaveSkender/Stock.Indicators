namespace Catalogging;

/// <summary>
/// Test class for Donchian catalog functionality.
/// </summary>
[TestClass]
public class DonchianTests : TestBase
{
    [TestMethod]
    public void DonchianSeriesListing()
    {
        // Act
        IndicatorListing listing = Donchian.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Donchian Channels");
        listing.Uiid.Should().Be("DONCHIAN");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceChannel);
        listing.MethodName.Should().Be("ToDonchian");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);

        IndicatorResult upperbandResult = listing.Results.SingleOrDefault(r => r.DataName == "UpperBand");
        upperbandResult.Should().NotBeNull();
        upperbandResult?.DisplayName.Should().Be("Upper Band");
        upperbandResult.IsReusable.Should().Be(false);
        IndicatorResult centerlineResult1 = listing.Results.SingleOrDefault(r => r.DataName == "Centerline");
        centerlineResult1.Should().NotBeNull();
        centerlineResult1?.DisplayName.Should().Be("Centerline");
        centerlineResult1.IsReusable.Should().Be(true);
        IndicatorResult lowerbandResult2 = listing.Results.SingleOrDefault(r => r.DataName == "LowerBand");
        lowerbandResult2.Should().NotBeNull();
        lowerbandResult2?.DisplayName.Should().Be("Lower Band");
        lowerbandResult2.IsReusable.Should().Be(false);
        IndicatorResult widthResult3 = listing.Results.SingleOrDefault(r => r.DataName == "Width");
        widthResult3.Should().NotBeNull();
        widthResult3?.DisplayName.Should().Be("Width");
        widthResult3.IsReusable.Should().Be(false);
    }
}
