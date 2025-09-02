namespace Catalog;

/// <summary>
/// Test class for WilliamsR catalog functionality.
/// </summary>
[TestClass]
public class WilliamsRTests : TestBase
{
    [TestMethod]
    public void WilliamsRSeriesListing()
    {
        // Act
        IndicatorListing listing = WilliamsR.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Williams %R");
        listing.Uiid.Should().Be("WILLR");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToWilliamsR");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult williamsrResult = listing.Results.SingleOrDefault(r => r.DataName == "WilliamsR");
        williamsrResult.Should().NotBeNull();
        williamsrResult?.DisplayName.Should().Be("Williams %R");
        williamsrResult.IsReusable.Should().Be(true);
    }
}
