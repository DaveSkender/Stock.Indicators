namespace Catalog;

/// <summary>
/// Test class for StarcBands catalog functionality.
/// </summary>
[TestClass]
public class StarcBandsTests : TestBase
{
    [TestMethod]
    public void StarcBandsSeriesListing()
    {
        // Act
        IndicatorListing listing = StarcBands.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("STARC Bands");
        listing.Uiid.Should().Be("STARC");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceChannel);
        listing.MethodName.Should().Be("ToStarcBands");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam smaPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "smaPeriods");
        smaPeriodsParam.Should().NotBeNull();
        IndicatorParam multiplierParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "multiplier");
        multiplierParam1.Should().NotBeNull();
        IndicatorParam atrPeriodsParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "atrPeriods");
        atrPeriodsParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(3);

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
    }
}
