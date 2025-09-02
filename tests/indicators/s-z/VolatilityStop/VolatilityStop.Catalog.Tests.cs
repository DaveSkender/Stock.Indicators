namespace Catalog;

/// <summary>
/// Test class for VolatilityStop catalog functionality.
/// </summary>
[TestClass]
public class VolatilityStopTests : TestBase
{
    [TestMethod]
    public void VolatilityStopSeriesListing()
    {
        // Act
        IndicatorListing listing = VolatilityStop.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Volatility Stop");
        listing.Uiid.Should().Be("VOL-STOP");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.StopAndReverse);
        listing.MethodName.Should().Be("ToVolatilityStop");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam multiplierParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "multiplier");
        multiplierParam1.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        IndicatorResult sarResult = listing.Results.SingleOrDefault(r => r.DataName == "Sar");
        sarResult.Should().NotBeNull();
        sarResult?.DisplayName.Should().Be("Stop and Reverse");
        sarResult.IsReusable.Should().Be(true);
        IndicatorResult isstopResult1 = listing.Results.SingleOrDefault(r => r.DataName == "IsStop");
        isstopResult1.Should().NotBeNull();
        isstopResult1?.DisplayName.Should().Be("Is Stop");
        isstopResult1.IsReusable.Should().Be(false);
    }
}
