namespace Catalogging;

/// <summary>
/// Test class for Cci catalog functionality.
/// </summary>
[TestClass]
public class CciTests : TestBase
{
    [TestMethod]
    public void CciSeriesListing()
    {
        // Act
        IndicatorListing listing = Cci.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Commodity Channel Index (CCI)");
        listing.Uiid.Should().Be("CCI");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToCci");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult cciResult = listing.Results.SingleOrDefault(r => r.DataName == "Cci");
        cciResult.Should().NotBeNull();
        cciResult?.DisplayName.Should().Be("CCI");
        cciResult.IsReusable.Should().Be(true);
    }
}
