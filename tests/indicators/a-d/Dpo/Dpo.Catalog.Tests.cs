namespace Catalog;

/// <summary>
/// Test class for Dpo catalog functionality.
/// </summary>
[TestClass]
public class DpoTests : TestBase
{
    [TestMethod]
    public void DpoSeriesListing()
    {
        // Act
        IndicatorListing listing = Dpo.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Detrended Price Oscillator");
        listing.Uiid.Should().Be("DPO");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToDpo");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        IndicatorResult dpoResult = listing.Results.SingleOrDefault(r => r.DataName == "Dpo");
        dpoResult.Should().NotBeNull();
        dpoResult?.DisplayName.Should().Be("DPO");
        dpoResult.IsReusable.Should().Be(true);
        IndicatorResult smaResult1 = listing.Results.SingleOrDefault(r => r.DataName == "Sma");
        smaResult1.Should().NotBeNull();
        smaResult1?.DisplayName.Should().Be("SMA");
        smaResult1.IsReusable.Should().Be(false);
    }
}
