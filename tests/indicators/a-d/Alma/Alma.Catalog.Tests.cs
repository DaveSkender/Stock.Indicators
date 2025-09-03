namespace Catalogging;

/// <summary>
/// Test class for Alma catalog functionality.
/// </summary>
[TestClass]
public class AlmaTests : TestBase
{
    [TestMethod]
    public void AlmaSeriesListing()
    {
        // Act
        IndicatorListing listing = Alma.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Arnaud Legoux Moving Average (ALMA)");
        listing.Uiid.Should().Be("ALMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToAlma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam offsetParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "offset");
        offsetParam1.Should().NotBeNull();
        IndicatorParam sigmaParam2 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "sigma");
        sigmaParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult almaResult = listing.Results.SingleOrDefault(r => r.DataName == "Alma");
        almaResult.Should().NotBeNull();
        almaResult?.DisplayName.Should().Be("Arnaud Legoux Moving Average (ALMA)");
        almaResult.IsReusable.Should().Be(true);
    }
}
