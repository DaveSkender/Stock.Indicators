namespace Catalogging;

/// <summary>
/// Test class for Doji catalog functionality.
/// </summary>
[TestClass]
public class DojiTests : TestBase
{
    [TestMethod]
    public void DojiSeriesListing()
    {
        // Act
        IndicatorListing listing = Doji.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Doji");
        listing.Uiid.Should().Be("DOJI");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.CandlestickPattern);
        listing.MethodName.Should().Be("ToDoji");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam maxPriceChangePercentParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "maxPriceChangePercent");
        maxPriceChangePercentParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult matchResult = listing.Results.SingleOrDefault(r => r.DataName == "Match");
        matchResult.Should().NotBeNull();
        matchResult?.DisplayName.Should().Be("Match");
        matchResult.IsReusable.Should().Be(true);
    }
}
