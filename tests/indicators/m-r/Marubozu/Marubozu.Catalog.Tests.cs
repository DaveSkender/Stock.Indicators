namespace Catalog;

/// <summary>
/// Test class for Marubozu catalog functionality.
/// </summary>
[TestClass]
public class MarubozuTests : TestBase
{
    [TestMethod]
    public void MarubozuSeriesListing()
    {
        // Act
        IndicatorListing listing = Marubozu.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Marubozu");
        listing.Uiid.Should().Be("MARUBOZU");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.CandlestickPattern);
        listing.MethodName.Should().Be("ToMarubozu");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam minBodyPercentParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "minBodyPercent");
        minBodyPercentParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult matchResult = listing.Results.SingleOrDefault(r => r.DataName == "Match");
        matchResult.Should().NotBeNull();
        matchResult?.DisplayName.Should().Be("Match");
        matchResult.IsReusable.Should().Be(true);
    }
}
