namespace Catalogging;

/// <summary>
/// Test class for Ichimoku catalog functionality.
/// </summary>
[TestClass]
public class IchimokuTests : TestBase
{
    [TestMethod]
    public void IchimokuSeriesListing()
    {
        // Act
        IndicatorListing listing = Ichimoku.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Ichimoku Cloud");
        listing.Uiid.Should().Be("ICHIMOKU");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceTrend);
        listing.MethodName.Should().Be("ToIchimoku");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam tenkanPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "tenkanPeriods");
        tenkanPeriodsParam.Should().NotBeNull();
        IndicatorParam kijunPeriodsParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "kijunPeriods");
        kijunPeriodsParam1.Should().NotBeNull();
        IndicatorParam senkouBPeriodsParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "senkouBPeriods");
        senkouBPeriodsParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(5);

        IndicatorResult tenkansenResult = listing.Results.SingleOrDefault(r => r.DataName == "TenkanSen");
        tenkansenResult.Should().NotBeNull();
        tenkansenResult?.DisplayName.Should().Be("Tenkan-sen");
        tenkansenResult.IsReusable.Should().Be(true);
        IndicatorResult kijunsenResult1 = listing.Results.SingleOrDefault(r => r.DataName == "KijunSen");
        kijunsenResult1.Should().NotBeNull();
        kijunsenResult1?.DisplayName.Should().Be("Kijun-sen");
        kijunsenResult1.IsReusable.Should().Be(false);
        IndicatorResult senkouspanaResult2 = listing.Results.SingleOrDefault(r => r.DataName == "SenkouSpanA");
        senkouspanaResult2.Should().NotBeNull();
        senkouspanaResult2?.DisplayName.Should().Be("Senkou Span A");
        senkouspanaResult2.IsReusable.Should().Be(false);
        IndicatorResult senkouspanbResult3 = listing.Results.SingleOrDefault(r => r.DataName == "SenkouSpanB");
        senkouspanbResult3.Should().NotBeNull();
        senkouspanbResult3?.DisplayName.Should().Be("Senkou Span B");
        senkouspanbResult3.IsReusable.Should().Be(false);
        IndicatorResult chikouspanResult4 = listing.Results.SingleOrDefault(r => r.DataName == "ChikouSpan");
        chikouspanResult4.Should().NotBeNull();
        chikouspanResult4?.DisplayName.Should().Be("Chikou Span");
        chikouspanResult4.IsReusable.Should().Be(false);
    }
}
