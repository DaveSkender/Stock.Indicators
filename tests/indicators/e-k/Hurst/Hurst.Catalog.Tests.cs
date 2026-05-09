namespace Catalogging;

/// <summary>
/// Test class for Hurst catalog functionality.
/// </summary>
[TestClass]
public class HurstTests : TestBase
{
    [TestMethod]
    public void HurstSeriesListing()
    {
        // Act
        IndicatorListing listing = Hurst.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Hurst Exponent");
        listing.Uiid.Should().Be("HURST");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceCharacteristic);
        listing.MethodName.Should().Be("ToHurst");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult hurstexponentResult = listing.Results.SingleOrDefault(static r => r.DataName == "HurstExponent");
        hurstexponentResult.Should().NotBeNull();
        hurstexponentResult?.DisplayName.Should().Be("Hurst Exponent");
        hurstexponentResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void HurstBufferListing()
    {
        // Act
        IndicatorListing listing = Hurst.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Hurst Exponent");
        listing.Uiid.Should().Be("HURST");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.PriceCharacteristic);
        listing.MethodName.Should().Be("ToHurstList");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult hurstexponentResult = listing.Results.SingleOrDefault(static r => r.DataName == "HurstExponent");
        hurstexponentResult.Should().NotBeNull();
        hurstexponentResult?.DisplayName.Should().Be("Hurst Exponent");
        hurstexponentResult.IsReusable.Should().Be(true);
    }
}
