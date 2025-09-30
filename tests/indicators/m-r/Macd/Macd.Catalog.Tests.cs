namespace Catalogging;

/// <summary>
/// Test class for Macd catalog functionality.
/// </summary>
[TestClass]
public class MacdTests : TestBase
{
    [TestMethod]
    public void MacdSeriesListing()
    {
        // Act
        IndicatorListing listing = Macd.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Moving Average Convergence/Divergence");
        listing.Uiid.Should().Be("MACD");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceTrend);
        listing.MethodName.Should().Be("ToMacd");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam fastPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "fastPeriods");
        fastPeriodsParam.Should().NotBeNull();
        IndicatorParam slowPeriodsParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "slowPeriods");
        slowPeriodsParam1.Should().NotBeNull();
        IndicatorParam signalPeriodsParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "signalPeriods");
        signalPeriodsParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(3);

        IndicatorResult macdResult = listing.Results.SingleOrDefault(r => r.DataName == "Macd");
        macdResult.Should().NotBeNull();
        macdResult?.DisplayName.Should().Be("MACD");
        macdResult.IsReusable.Should().Be(true);
        IndicatorResult signalResult1 = listing.Results.SingleOrDefault(r => r.DataName == "Signal");
        signalResult1.Should().NotBeNull();
        signalResult1?.DisplayName.Should().Be("Signal");
        signalResult1.IsReusable.Should().Be(false);
    }

    [TestMethod]
    public void MacdBufferListing()
    {
        // Act
        IndicatorListing listing = Macd.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Moving Average Convergence/Divergence");
        listing.Uiid.Should().Be("MACD");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.PriceTrend);
        listing.MethodName.Should().Be("ToMacd");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam fastPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "fastPeriods");
        fastPeriodsParam.Should().NotBeNull();
        fastPeriodsParam?.DefaultValue.Should().Be(12);
        IndicatorParam slowPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "slowPeriods");
        slowPeriodsParam.Should().NotBeNull();
        slowPeriodsParam?.DefaultValue.Should().Be(26);
        IndicatorParam signalPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "signalPeriods");
        signalPeriodsParam.Should().NotBeNull();
        signalPeriodsParam?.DefaultValue.Should().Be(9);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(3);

        IndicatorResult macdResult = listing.Results.SingleOrDefault(r => r.DataName == "Macd");
        macdResult.Should().NotBeNull();
        macdResult?.DisplayName.Should().Be("MACD");
        macdResult.IsReusable.Should().Be(true);
        IndicatorResult signalResult = listing.Results.SingleOrDefault(r => r.DataName == "Signal");
        signalResult.Should().NotBeNull();
        signalResult?.DisplayName.Should().Be("Signal");
        signalResult.IsReusable.Should().Be(false);
        IndicatorResult histogramResult = listing.Results.SingleOrDefault(r => r.DataName == "Histogram");
        histogramResult.Should().NotBeNull();
        histogramResult?.DisplayName.Should().Be("Histogram");
        histogramResult.IsReusable.Should().Be(false);
    }
}
