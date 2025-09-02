namespace Catalogging;

/// <summary>
/// Test class for Smi catalog functionality.
/// </summary>
[TestClass]
public class SmiTests : TestBase
{
    [TestMethod]
    public void SmiSeriesListing()
    {
        // Act
        IndicatorListing listing = Smi.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Stochastic Momentum Index");
        listing.Uiid.Should().Be("SMI");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToSmi");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(4);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam firstSmoothPeriodsParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "firstSmoothPeriods");
        firstSmoothPeriodsParam1.Should().NotBeNull();
        IndicatorParam secondSmoothPeriodsParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "secondSmoothPeriods");
        secondSmoothPeriodsParam2.Should().NotBeNull();
        IndicatorParam signalPeriodsParam3 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "signalPeriods");
        signalPeriodsParam3.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        IndicatorResult smiResult = listing.Results.SingleOrDefault(r => r.DataName == "Smi");
        smiResult.Should().NotBeNull();
        smiResult?.DisplayName.Should().Be("SMI");
        smiResult.IsReusable.Should().Be(true);
        IndicatorResult signalResult1 = listing.Results.SingleOrDefault(r => r.DataName == "Signal");
        signalResult1.Should().NotBeNull();
        signalResult1?.DisplayName.Should().Be("Signal");
        signalResult1.IsReusable.Should().Be(false);
    }
}
