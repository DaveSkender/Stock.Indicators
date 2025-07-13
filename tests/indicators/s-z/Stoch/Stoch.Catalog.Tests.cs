namespace Catalog;

[TestClass]
public class StochTests : TestBase
{
    [TestMethod]
    public void StochSeriesListing()
    {
        // Act
        var listing = Stoch.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Stochastic Oscillator");
        listing.Uiid.Should().Be("STOCH");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        var lookbackPeriods = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriods.Should().NotBeNull();
        lookbackPeriods!.DisplayName.Should().Be("Lookback Periods");
        lookbackPeriods.Description.Should().Be("Number of periods for the stochastic calculation");
        lookbackPeriods.IsRequired.Should().BeFalse();

        var signalPeriods = listing.Parameters.FirstOrDefault(p => p.ParameterName == "signalPeriods");
        signalPeriods.Should().NotBeNull();
        signalPeriods!.DisplayName.Should().Be("Signal Periods");
        signalPeriods.Description.Should().Be("Number of periods for the signal line");
        signalPeriods.IsRequired.Should().BeFalse();

        var smoothPeriods = listing.Parameters.FirstOrDefault(p => p.ParameterName == "smoothPeriods");
        smoothPeriods.Should().NotBeNull();
        smoothPeriods!.DisplayName.Should().Be("Smooth Periods");
        smoothPeriods.Description.Should().Be("Number of periods for smoothing");
        smoothPeriods.IsRequired.Should().BeFalse();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        var oscillator = listing.Results[0];
        oscillator.DataName.Should().Be("Oscillator");
        oscillator.DisplayName.Should().Be("%K");
        oscillator.DataType.Should().Be(ResultType.Default);
        oscillator.IsDefault.Should().BeTrue();

        var signal = listing.Results[1];
        signal.DataName.Should().Be("Signal");
        signal.DisplayName.Should().Be("%D");
        signal.DataType.Should().Be(ResultType.Default);
        signal.IsDefault.Should().BeFalse();
    }
}
