namespace Catalog;

[TestClass]
public class RsiTests : TestBase
{
    [TestMethod]
    public void RsiSeriesListing()
    {
        // Act
        var listing = Rsi.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Relative Strength Index");
        listing.Uiid.Should().Be("RSI");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var lookbackPeriods = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriods.Should().NotBeNull();
        lookbackPeriods!.DisplayName.Should().Be("Lookback Periods");
        lookbackPeriods.Description.Should().Be("Number of periods for the RSI calculation");
        lookbackPeriods.IsRequired.Should().BeFalse();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Rsi");
        result.DisplayName.Should().Be("RSI");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }
}