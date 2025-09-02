namespace Catalog;

/// <summary>
/// Test class for T3 catalog functionality.
/// </summary>
[TestClass]
public class T3Tests : TestBase
{
    [TestMethod]
    public void T3SeriesListing()
    {
        // Act
        IndicatorListing listing = T3.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("T3 Moving Average");
        listing.Uiid.Should().Be("T3");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToT3");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam volumeFactorParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "volumeFactor");
        volumeFactorParam1.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult t3Result = listing.Results.SingleOrDefault(r => r.DataName == "T3");
        t3Result.Should().NotBeNull();
        t3Result?.DisplayName.Should().Be("T3");
        t3Result.IsReusable.Should().Be(true);
    }
}
