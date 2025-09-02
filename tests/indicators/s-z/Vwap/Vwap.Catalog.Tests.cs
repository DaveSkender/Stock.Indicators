namespace Catalog;

/// <summary>
/// Test class for Vwap catalog functionality.
/// </summary>
[TestClass]
public class VwapTests : TestBase
{
    [TestMethod]
    public void VwapSeriesListing()
    {
        // Act
        IndicatorListing listing = Vwap.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Volume Weighted Average Price");
        listing.Uiid.Should().Be("VWAP");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceChannel);
        listing.MethodName.Should().Be("ToVwap");

        listing.Parameters?.Count.Should().Be(1);
        // No parameters for this indicator

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(3);

        IndicatorResult vwapResult = listing.Results.SingleOrDefault(r => r.DataName == "Vwap");
        vwapResult.Should().NotBeNull();
        vwapResult?.DisplayName.Should().Be("VWAP");
        vwapResult.IsReusable.Should().Be(true);
        IndicatorResult upperbandResult1 = listing.Results.SingleOrDefault(r => r.DataName == "UpperBand");
        upperbandResult1.Should().NotBeNull();
        upperbandResult1?.DisplayName.Should().Be("Upper Band");
        upperbandResult1.IsReusable.Should().Be(false);
        IndicatorResult lowerbandResult2 = listing.Results.SingleOrDefault(r => r.DataName == "LowerBand");
        lowerbandResult2.Should().NotBeNull();
        lowerbandResult2?.DisplayName.Should().Be("Lower Band");
        lowerbandResult2.IsReusable.Should().Be(false);
    }
}
