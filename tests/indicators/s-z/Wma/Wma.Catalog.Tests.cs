namespace Catalog;

/// <summary>
/// Test class for Wma catalog functionality.
/// </summary>
[TestClass]
public class WmaTests : TestBase
{
    [TestMethod]
    public void WmaSeriesListing()
    {
        // Act
        IndicatorListing listing = Wma.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Weighted Moving Average");
        listing.Uiid.Should().Be("WMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToWma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult wmaResult = listing.Results.SingleOrDefault(r => r.DataName == "Wma");
        wmaResult.Should().NotBeNull();
        wmaResult!.DisplayName.Should().Be("WMA");
        wmaResult.IsReusable.Should().Be(true);
    }
}
