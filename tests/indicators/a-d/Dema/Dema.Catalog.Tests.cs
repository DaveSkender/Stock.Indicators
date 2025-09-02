namespace Catalogging;

/// <summary>
/// Test class for Dema catalog functionality.
/// </summary>
[TestClass]
public class DemaTests : TestBase
{
    [TestMethod]
    public void DemaSeriesListing()
    {
        // Act
        IndicatorListing listing = Dema.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Double Exponential Moving Average");
        listing.Uiid.Should().Be("DEMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToDema");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult demaResult = listing.Results.SingleOrDefault(r => r.DataName == "Dema");
        demaResult.Should().NotBeNull();
        demaResult?.DisplayName.Should().Be("DEMA");
        demaResult.IsReusable.Should().Be(true);
    }
}
