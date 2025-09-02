namespace Catalog;

/// <summary>
/// Test class for SmaAnalysis catalog functionality.
/// </summary>
[TestClass]
public class SmaAnalysisTests : TestBase
{
    [TestMethod]
    public void SmaAnalysisSeriesListing()
    {
        // Act
        IndicatorListing listing = SmaAnalysis.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Simple Moving Average Analysis");
        listing.Uiid.Should().Be("SMA-ANALYSIS");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceCharacteristic);
        listing.MethodName.Should().Be("ToSmaAnalysis");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);

        IndicatorResult smaResult = listing.Results.SingleOrDefault(r => r.DataName == "Sma");
        smaResult.Should().NotBeNull();
        smaResult?.DisplayName.Should().Be("SMA");
        smaResult.IsReusable.Should().Be(true);
        IndicatorResult madResult1 = listing.Results.SingleOrDefault(r => r.DataName == "Mad");
        madResult1.Should().NotBeNull();
        madResult1?.DisplayName.Should().Be("Mean Absolute Deviation");
        madResult1.IsReusable.Should().Be(false);
        IndicatorResult mseResult2 = listing.Results.SingleOrDefault(r => r.DataName == "Mse");
        mseResult2.Should().NotBeNull();
        mseResult2?.DisplayName.Should().Be("Mean Square Error");
        mseResult2.IsReusable.Should().Be(false);
        IndicatorResult mapeResult3 = listing.Results.SingleOrDefault(r => r.DataName == "Mape");
        mapeResult3.Should().NotBeNull();
        mapeResult3?.DisplayName.Should().Be("Mean Absolute Percentage Error");
        mapeResult3.IsReusable.Should().Be(false);
    }
}
