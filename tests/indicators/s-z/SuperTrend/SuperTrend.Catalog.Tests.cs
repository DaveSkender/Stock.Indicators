namespace Catalogging;

/// <summary>
/// Test class for SuperTrend catalog functionality.
/// </summary>
[TestClass]
public class SuperTrendTests : TestBase
{
    [TestMethod]
    public void SuperTrendSeriesListing()
    {
        // Act
        IndicatorListing listing = SuperTrend.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("SuperTrend");
        listing.Uiid.Should().Be("SUPERTREND");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceTrend);
        listing.MethodName.Should().Be("ToSuperTrend");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam multiplierParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "multiplier");
        multiplierParam1.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(3);

        IndicatorResult supertrendResult = listing.Results.SingleOrDefault(r => r.DataName == "SuperTrend");
        supertrendResult.Should().NotBeNull();
        supertrendResult?.DisplayName.Should().Be("SuperTrend");
        supertrendResult.IsReusable.Should().Be(true);
        IndicatorResult upperbandResult1 = listing.Results.SingleOrDefault(r => r.DataName == "UpperBand");
        upperbandResult1.Should().NotBeNull();
        upperbandResult1?.DisplayName.Should().Be("Upper Band");
        upperbandResult1.IsReusable.Should().Be(false);
        IndicatorResult lowerbandResult2 = listing.Results.SingleOrDefault(r => r.DataName == "LowerBand");
        lowerbandResult2.Should().NotBeNull();
        lowerbandResult2?.DisplayName.Should().Be("Lower Band");
        lowerbandResult2.IsReusable.Should().Be(false);
    }
}
