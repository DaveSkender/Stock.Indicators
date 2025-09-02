namespace Catalog;

/// <summary>
/// Test class for Roc catalog functionality.
/// </summary>
[TestClass]
public class RocTests : TestBase
{
    [TestMethod]
    public void RocSeriesListing()
    {
        // Act
        IndicatorListing listing = Roc.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Rate of Change");
        listing.Uiid.Should().Be("ROC");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToRoc");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult rocResult = listing.Results.SingleOrDefault(r => r.DataName == "Roc");
        rocResult.Should().NotBeNull();
        rocResult?.DisplayName.Should().Be("ROC");
        rocResult.IsReusable.Should().Be(true);
    }
}
