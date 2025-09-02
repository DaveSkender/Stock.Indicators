namespace Catalogging;

/// <summary>
/// Test class for Ultimate catalog functionality.
/// </summary>
[TestClass]
public class UltimateTests : TestBase
{
    [TestMethod]
    public void UltimateSeriesListing()
    {
        // Act
        IndicatorListing listing = Ultimate.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Ultimate Oscillator");
        listing.Uiid.Should().Be("UO");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToUltimate");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam shortPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "shortPeriods");
        shortPeriodsParam.Should().NotBeNull();
        IndicatorParam middlePeriodsParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "middlePeriods");
        middlePeriodsParam1.Should().NotBeNull();
        IndicatorParam longPeriodsParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "longPeriods");
        longPeriodsParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult ultimateResult = listing.Results.SingleOrDefault(r => r.DataName == "Ultimate");
        ultimateResult.Should().NotBeNull();
        ultimateResult?.DisplayName.Should().Be("Ultimate Oscillator");
        ultimateResult.IsReusable.Should().Be(true);
    }
}
