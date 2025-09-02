namespace Catalogging;

/// <summary>
/// Test class for ForceIndex catalog functionality.
/// </summary>
[TestClass]
public class ForceIndexTests : TestBase
{
    [TestMethod]
    public void ForceIndexSeriesListing()
    {
        // Act
        IndicatorListing listing = ForceIndex.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Force Index");
        listing.Uiid.Should().Be("FORCE");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.VolumeBased);
        listing.MethodName.Should().Be("ToForceIndex");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult forceindexResult = listing.Results.SingleOrDefault(r => r.DataName == "ForceIndex");
        forceindexResult.Should().NotBeNull();
        forceindexResult?.DisplayName.Should().Be("Force Index");
        forceindexResult.IsReusable.Should().Be(true);
    }
}
