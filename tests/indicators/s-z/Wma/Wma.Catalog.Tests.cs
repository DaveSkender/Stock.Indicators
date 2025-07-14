namespace Catalog;

[TestClass]
public class WmaTests : TestBase
{
    [TestMethod]
    public void WmaSeriesListing()
    {
        // Act
        var listing = Wma.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Weighted Moving Average");
        listing.Uiid.Should().Be("WMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var lookbackParam = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackParam.Should().NotBeNull();
        lookbackParam!.DisplayName.Should().Be("Lookback Periods");
        lookbackParam.Description.Should().Be("Number of periods for the WMA calculation");
        lookbackParam.IsRequired.Should().BeTrue();
        lookbackParam.DataType.Should().Be("Int32");
        lookbackParam.DefaultValue.Should().Be(14);
        lookbackParam.Minimum.Should().Be(1);
        lookbackParam.Maximum.Should().Be(250);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Wma");
        result.DisplayName.Should().Be("WMA");
        result.DataType.Should().Be(ResultType.Default);
        result.IsReusable.Should().BeTrue();
    }
}
