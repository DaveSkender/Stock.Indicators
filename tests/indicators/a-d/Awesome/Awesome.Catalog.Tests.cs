namespace Catalog;

/// <summary>
/// Test class for Awesome catalog functionality.
/// </summary>
[TestClass]
public class AwesomeTests : TestBase
{
    [TestMethod]
    public void AwesomeSeriesListing()
    {
        // Act
        IndicatorListing listing = Awesome.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Awesome Oscillator");
        listing.Uiid.Should().Be("AWESOME");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToAwesome");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        IndicatorParam fastPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "fastPeriods");
        fastPeriodsParam.Should().NotBeNull();
        IndicatorParam slowPeriodsParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "slowPeriods");
        slowPeriodsParam1.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(2);

        IndicatorResult oscillatorResult = listing.Results.SingleOrDefault(r => r.DataName == "Oscillator");
        oscillatorResult.Should().NotBeNull();
        oscillatorResult?.DisplayName.Should().Be("Oscillator");
        oscillatorResult.IsReusable.Should().Be(true);
        IndicatorResult normalizedResult1 = listing.Results.SingleOrDefault(r => r.DataName == "Normalized");
        normalizedResult1.Should().NotBeNull();
        normalizedResult1?.DisplayName.Should().Be("Normalized");
        normalizedResult1.IsReusable.Should().Be(false);
    }
}
