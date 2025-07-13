namespace Catalog;

[TestClass]
public class SmaTests : TestBase
{
    [TestMethod]
    public void SmaSeriesListing()
    {
        // Act
        var listing = Sma.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Simple Moving Average");
        listing.Uiid.Should().Be("SMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var lookbackParam = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackParam.Should().NotBeNull();
        lookbackParam!.DisplayName.Should().Be("Lookback Periods");
        lookbackParam.Description.Should().Be("Number of periods for the SMA calculation");
        lookbackParam.IsRequired.Should().BeTrue();
        lookbackParam.DataType.Should().Be("Int32");
        lookbackParam.DefaultValue.Should().Be(20);
        lookbackParam.Minimum.Should().Be(1);
        lookbackParam.Maximum.Should().Be(250);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Sma");
        result.DisplayName.Should().Be("SMA");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }

    [TestMethod]
    public void SmaStreamListing()
    {
        // Act
        var listing = Sma.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Simple Moving Average");
        listing.Uiid.Should().Be("SMA");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var lookbackParam = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackParam.Should().NotBeNull();
        lookbackParam!.DisplayName.Should().Be("Lookback Periods");
        lookbackParam.Description.Should().Be("Number of periods for the SMA calculation");
        lookbackParam.IsRequired.Should().BeTrue();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Sma");
        result.DisplayName.Should().Be("SMA");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }
}