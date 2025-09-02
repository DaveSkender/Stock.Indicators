namespace Catalog;

/// <summary>
/// Test class for RocWb catalog functionality.
/// </summary>
[TestClass]
public class RocWbTests : TestBase
{
    [TestMethod]
    public void RocWbSeriesListing()
    {
        // Act
        IndicatorListing listing = RocWb.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("ROC with Bands");
        listing.Uiid.Should().Be("ROC-WB");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceTrend);
        listing.MethodName.Should().Be("ToRocWb");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam emaPeriodsParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "emaPeriods");
        emaPeriodsParam1.Should().NotBeNull();
        IndicatorParam stdDevPeriodsParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "stdDevPeriods");
        stdDevPeriodsParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);

        IndicatorResult rocResult = listing.Results.SingleOrDefault(r => r.DataName == "Roc");
        rocResult.Should().NotBeNull();
        rocResult?.DisplayName.Should().Be("ROC");
        rocResult.IsReusable.Should().Be(true);
        IndicatorResult rocemaResult1 = listing.Results.SingleOrDefault(r => r.DataName == "RocEma");
        rocemaResult1.Should().NotBeNull();
        rocemaResult1?.DisplayName.Should().Be("ROC EMA");
        rocemaResult1.IsReusable.Should().Be(false);
        IndicatorResult upperbandResult2 = listing.Results.SingleOrDefault(r => r.DataName == "UpperBand");
        upperbandResult2.Should().NotBeNull();
        upperbandResult2?.DisplayName.Should().Be("Upper Band");
        upperbandResult2.IsReusable.Should().Be(false);
        IndicatorResult lowerbandResult3 = listing.Results.SingleOrDefault(r => r.DataName == "LowerBand");
        lowerbandResult3.Should().NotBeNull();
        lowerbandResult3?.DisplayName.Should().Be("Lower Band");
        lowerbandResult3.IsReusable.Should().Be(false);
    }
}
