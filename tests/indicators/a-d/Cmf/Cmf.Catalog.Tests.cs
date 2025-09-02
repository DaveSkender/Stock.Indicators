namespace Catalogging;

/// <summary>
/// Test class for Cmf catalog functionality.
/// </summary>
[TestClass]
public class CmfTests : TestBase
{
    [TestMethod]
    public void CmfSeriesListing()
    {
        // Act
        IndicatorListing listing = Cmf.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Chaikin Money Flow (CMF)");
        listing.Uiid.Should().Be("CMF");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.VolumeBased);
        listing.MethodName.Should().Be("ToCmf");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(3);

        IndicatorResult moneyflowmultiplierResult = listing.Results.SingleOrDefault(r => r.DataName == "MoneyFlowMultiplier");
        moneyflowmultiplierResult.Should().NotBeNull();
        moneyflowmultiplierResult?.DisplayName.Should().Be("Money Flow Multiplier");
        moneyflowmultiplierResult.IsReusable.Should().Be(false);
        IndicatorResult moneyflowvolumeResult1 = listing.Results.SingleOrDefault(r => r.DataName == "MoneyFlowVolume");
        moneyflowvolumeResult1.Should().NotBeNull();
        moneyflowvolumeResult1?.DisplayName.Should().Be("Money Flow Volume");
        moneyflowvolumeResult1.IsReusable.Should().Be(false);
        IndicatorResult cmfResult2 = listing.Results.SingleOrDefault(r => r.DataName == "Cmf");
        cmfResult2.Should().NotBeNull();
        cmfResult2?.DisplayName.Should().Be("CMF");
        cmfResult2.IsReusable.Should().Be(true);
    }
}
