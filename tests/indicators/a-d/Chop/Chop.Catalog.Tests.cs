namespace Catalogging;

/// <summary>
/// Test class for Chop catalog functionality.
/// </summary>
[TestClass]
public class ChopTests : TestBase
{
    [TestMethod]
    public void ChopSeriesListing()
    {
        // Act
        IndicatorListing listing = Chop.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Choppiness Index");
        listing.Uiid.Should().Be("CHOP");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToChop");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult chopResult = listing.Results.SingleOrDefault(r => r.DataName == "Chop");
        chopResult.Should().NotBeNull();
        chopResult?.DisplayName.Should().Be("CHOP");
        chopResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void ChopStreamListing()
    {
        // Act
        IndicatorListing listing = Chop.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Choppiness Index");
        listing.Uiid.Should().Be("CHOP");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToChop");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult chopResult = listing.Results.SingleOrDefault(r => r.DataName == "Chop");
        chopResult.Should().NotBeNull();
        chopResult?.DisplayName.Should().Be("CHOP");
        chopResult.IsReusable.Should().Be(true);
    }
}
