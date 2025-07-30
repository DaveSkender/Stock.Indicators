namespace Catalog;

/// <summary>
/// Test class for Obv catalog functionality.
/// </summary>
[TestClass]
public class ObvTests : TestBase
{
    [TestMethod]
    public void ObvSeriesListing()
    {
        // Act
        IndicatorListing listing = Obv.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("On-Balance Volume");
        listing.Uiid.Should().Be("OBV");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.VolumeBased);
        listing.MethodName.Should().Be("ToObv");

        listing.Parameters?.Count.Should().Be(0);
        // No parameters for this indicator

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult obvResult = listing.Results.SingleOrDefault(r => r.DataName == "Obv");
        obvResult.Should().NotBeNull();
        obvResult!.DisplayName.Should().Be("OBV");
        obvResult.IsReusable.Should().Be(true);
    }
}
