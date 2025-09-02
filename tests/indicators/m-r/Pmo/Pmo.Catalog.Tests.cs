namespace Catalogging;

/// <summary>
/// Test class for Pmo catalog functionality.
/// </summary>
[TestClass]
public class PmoTests : TestBase
{
    [TestMethod]
    public void PmoSeriesListing()
    {
        // Act
        IndicatorListing listing = Pmo.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Price Momentum Oscillator");
        listing.Uiid.Should().Be("PMO");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToPmo");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam timePeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "timePeriods");
        timePeriodsParam.Should().NotBeNull();
        IndicatorParam smoothingPeriodsParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "smoothingPeriods");
        smoothingPeriodsParam1.Should().NotBeNull();
        IndicatorParam signalPeriodsParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "signalPeriods");
        signalPeriodsParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        IndicatorResult pmoResult = listing.Results.SingleOrDefault(r => r.DataName == "Pmo");
        pmoResult.Should().NotBeNull();
        pmoResult?.DisplayName.Should().Be("PMO");
        pmoResult.IsReusable.Should().Be(true);
        IndicatorResult signalResult1 = listing.Results.SingleOrDefault(r => r.DataName == "Signal");
        signalResult1.Should().NotBeNull();
        signalResult1?.DisplayName.Should().Be("Signal");
        signalResult1.IsReusable.Should().Be(false);
    }
}
