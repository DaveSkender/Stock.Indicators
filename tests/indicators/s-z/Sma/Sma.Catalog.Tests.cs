namespace Catalog;

/// <summary>
/// Test class for Sma catalog functionality.
/// </summary>
[TestClass]
public class SmaTests : TestBase
{
    [TestMethod]
    public void SmaSeriesListing()
    {
        // Act
        IndicatorListing listing = Sma.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Simple Moving Average");
        listing.Uiid.Should().Be("SMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToSma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult smaResult = listing.Results.SingleOrDefault(r => r.DataName == "Sma");
        smaResult.Should().NotBeNull();
        smaResult!.DisplayName.Should().Be("SMA");
        smaResult.IsReusable.Should().Be(true);
    }
    [TestMethod]
    public void SmaStreamListing()
    {
        // Act
        IndicatorListing listing = Sma.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Simple Moving Average");
        listing.Uiid.Should().Be("SMA");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToSma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult smaResult = listing.Results.SingleOrDefault(r => r.DataName == "Sma");
        smaResult.Should().NotBeNull();
        smaResult!.DisplayName.Should().Be("SMA");
        smaResult.IsReusable.Should().Be(true);
    }
}
