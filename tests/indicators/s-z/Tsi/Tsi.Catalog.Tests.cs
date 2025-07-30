namespace Catalog;

/// <summary>
/// Test class for Tsi catalog functionality.
/// </summary>
[TestClass]
public class TsiTests : TestBase
{
    [TestMethod]
    public void TsiSeriesListing()
    {
        // Act
        IndicatorListing listing = Tsi.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("True Strength Index");
        listing.Uiid.Should().Be("TSI");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToTsi");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam smoothPeriodsParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "smoothPeriods");
        smoothPeriodsParam1.Should().NotBeNull();
        IndicatorParam signalPeriodsParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "signalPeriods");
        signalPeriodsParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        IndicatorResult tsiResult = listing.Results.SingleOrDefault(r => r.DataName == "Tsi");
        tsiResult.Should().NotBeNull();
        tsiResult!.DisplayName.Should().Be("TSI");
        tsiResult.IsReusable.Should().Be(true);
        IndicatorResult signalResult1 = listing.Results.SingleOrDefault(r => r.DataName == "Signal");
        signalResult1.Should().NotBeNull();
        signalResult1!.DisplayName.Should().Be("Signal");
        signalResult1.IsReusable.Should().Be(false);
    }
}
