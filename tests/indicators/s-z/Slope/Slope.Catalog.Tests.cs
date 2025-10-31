namespace Catalogging;

/// <summary>
/// Test class for Slope catalog functionality.
/// </summary>
[TestClass]
public class SlopeTests : TestBase
{
    [TestMethod]
    public void SlopeSeriesListing()
    {
        // Act
        IndicatorListing listing = Slope.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Slope");
        listing.Uiid.Should().Be("SLOPE");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceCharacteristic);
        listing.MethodName.Should().Be("ToSlope");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);

        IndicatorResult slopeResult = listing.Results.SingleOrDefault(static r => r.DataName == "Slope");
        slopeResult.Should().NotBeNull();
        slopeResult?.DisplayName.Should().Be("Slope");
        slopeResult.IsReusable.Should().Be(true);
        IndicatorResult interceptResult1 = listing.Results.SingleOrDefault(static r => r.DataName == "Intercept");
        interceptResult1.Should().NotBeNull();
        interceptResult1?.DisplayName.Should().Be("Intercept");
        interceptResult1.IsReusable.Should().Be(false);
        IndicatorResult stddevResult2 = listing.Results.SingleOrDefault(static r => r.DataName == "StdDev");
        stddevResult2.Should().NotBeNull();
        stddevResult2?.DisplayName.Should().Be("Standard Deviation");
        stddevResult2.IsReusable.Should().Be(false);
        IndicatorResult rsquaredResult3 = listing.Results.SingleOrDefault(static r => r.DataName == "RSquared");
        rsquaredResult3.Should().NotBeNull();
        rsquaredResult3?.DisplayName.Should().Be("R-Squared");
        rsquaredResult3.IsReusable.Should().Be(false);
    }

    [TestMethod]
    public void SlopeStreamListing()
    {
        // Act
        IndicatorListing listing = Slope.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Slope");
        listing.Uiid.Should().Be("SLOPE");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.PriceCharacteristic);
        listing.MethodName.Should().Be("ToSlope");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);

        IndicatorResult slopeResult = listing.Results.SingleOrDefault(static r => r.DataName == "Slope");
        slopeResult.Should().NotBeNull();
        slopeResult!.DisplayName.Should().Be("Slope");
        slopeResult!.IsReusable.Should().Be(true);
        IndicatorResult interceptResult1 = listing.Results.SingleOrDefault(static r => r.DataName == "Intercept");
        interceptResult1.Should().NotBeNull();
        interceptResult1!.DisplayName.Should().Be("Intercept");
        interceptResult1!.IsReusable.Should().Be(false);
        IndicatorResult stddevResult2 = listing.Results.SingleOrDefault(static r => r.DataName == "StdDev");
        stddevResult2.Should().NotBeNull();
        stddevResult2!.DisplayName.Should().Be("Standard Deviation");
        stddevResult2!.IsReusable.Should().Be(false);
        IndicatorResult rsquaredResult3 = listing.Results.SingleOrDefault(static r => r.DataName == "RSquared");
        rsquaredResult3.Should().NotBeNull();
        rsquaredResult3!.DisplayName.Should().Be("R-Squared");
        rsquaredResult3!.IsReusable.Should().Be(false);
    }
}
