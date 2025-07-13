namespace Catalog;

[TestClass]
public class SmaAnalysisTests : TestBase
{
    [TestMethod]
    public void SmaAnalysisSeriesListing()
    {
        // Act
        var listing = SmaAnalysis.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Simple Moving Average Analysis");
        listing.Uiid.Should().Be("SMA-ANALYSIS");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceCharacteristic);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var lookbackParam = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackParam.Should().NotBeNull();
        lookbackParam!.DisplayName.Should().Be("Lookback Periods");
        lookbackParam.Description.Should().Be("Number of periods for the SMA analysis");
        lookbackParam.IsRequired.Should().BeTrue();
        lookbackParam.DataType.Should().Be("Int32");
        lookbackParam.DefaultValue.Should().Be(20);
        lookbackParam.Minimum.Should().Be(1);
        lookbackParam.Maximum.Should().Be(250);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);

        var smaResult = listing.Results.FirstOrDefault(r => r.DataName == "Sma");
        smaResult.Should().NotBeNull();
        smaResult!.DisplayName.Should().Be("SMA");
        smaResult.DataType.Should().Be(ResultType.Default);
        smaResult.IsDefault.Should().BeTrue();

        var madResult = listing.Results.FirstOrDefault(r => r.DataName == "Mad");
        madResult.Should().NotBeNull();
        madResult!.DisplayName.Should().Be("Mean Absolute Deviation");
        madResult.DataType.Should().Be(ResultType.Default);
        madResult.IsDefault.Should().BeFalse();

        var mseResult = listing.Results.FirstOrDefault(r => r.DataName == "Mse");
        mseResult.Should().NotBeNull();
        mseResult!.DisplayName.Should().Be("Mean Square Error");
        mseResult.DataType.Should().Be(ResultType.Default);
        mseResult.IsDefault.Should().BeFalse();

        var mapeResult = listing.Results.FirstOrDefault(r => r.DataName == "Mape");
        mapeResult.Should().NotBeNull();
        mapeResult!.DisplayName.Should().Be("Mean Absolute Percentage Error");
        mapeResult.DataType.Should().Be(ResultType.Default);
        mapeResult.IsDefault.Should().BeFalse();
    }
}
