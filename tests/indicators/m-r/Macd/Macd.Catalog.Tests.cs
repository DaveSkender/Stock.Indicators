namespace Catalog;

[TestClass]
public class MacdTests : TestBase
{
    [TestMethod]
    public void MacdListing()
    {
        // Act
        var listing = Macd.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Moving Average Convergence/Divergence");
        listing.Uiid.Should().Be("MACD");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceTrend);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        var fastPeriods = listing.Parameters.FirstOrDefault(p => p.ParameterName == "fastPeriods");
        fastPeriods.Should().NotBeNull();
        fastPeriods!.DisplayName.Should().Be("Fast Periods");
        fastPeriods.Description.Should().Be("Number of periods for the fast EMA");
        fastPeriods.IsRequired.Should().BeFalse();
        fastPeriods.DefaultValue.Should().Be(12);

        var slowPeriods = listing.Parameters.FirstOrDefault(p => p.ParameterName == "slowPeriods");
        slowPeriods.Should().NotBeNull();
        slowPeriods!.DisplayName.Should().Be("Slow Periods");
        slowPeriods.Description.Should().Be("Number of periods for the slow EMA");
        slowPeriods.IsRequired.Should().BeFalse();
        slowPeriods.DefaultValue.Should().Be(26);

        var signalPeriods = listing.Parameters.FirstOrDefault(p => p.ParameterName == "signalPeriods");
        signalPeriods.Should().NotBeNull();
        signalPeriods!.DisplayName.Should().Be("Signal Periods");
        signalPeriods.Description.Should().Be("Number of periods for the signal line");
        signalPeriods.IsRequired.Should().BeFalse();
        signalPeriods.DefaultValue.Should().Be(9);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(3);

        var macdResult = listing.Results[0];
        macdResult.DataName.Should().Be("Macd");
        macdResult.DisplayName.Should().Be("MACD");
        macdResult.DataType.Should().Be(ResultType.Default);
        macdResult.IsReusable.Should().BeTrue();

        var signalResult = listing.Results[1];
        signalResult.DataName.Should().Be("Signal");
        signalResult.DisplayName.Should().Be("Signal");
        signalResult.DataType.Should().Be(ResultType.Default);
        signalResult.IsReusable.Should().BeFalse();

        var histogramResult = listing.Results[2];
        histogramResult.DataName.Should().Be("Histogram");
        histogramResult.DisplayName.Should().Be("Histogram");
        histogramResult.DataType.Should().Be(ResultType.Bar);
        histogramResult.IsReusable.Should().BeFalse();
    }
}
