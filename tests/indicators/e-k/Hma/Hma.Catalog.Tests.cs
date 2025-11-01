namespace Catalogging;

/// <summary>
/// Test class for Hma catalog functionality.
/// </summary>
[TestClass]
public class HmaTests : TestBase
{
    [TestMethod]
    public void HmaSeriesListing()
    {
        // Act
        IndicatorListing listing = Hma.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Hull Moving Average");
        listing.Uiid.Should().Be("HMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToHma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult hmaResult = listing.Results.SingleOrDefault(static r => r.DataName == "Hma");
        hmaResult.Should().NotBeNull();
        hmaResult?.DisplayName.Should().Be("HMA");
        hmaResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void HmaStreamListing()
    {
        // Act
        IndicatorListing listing = Hma.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Hull Moving Average");
        listing.Uiid.Should().Be("HMA");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToHma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult hmaResult = listing.Results.SingleOrDefault(static r => r.DataName == "Hma");
        hmaResult.Should().NotBeNull();
        hmaResult?.DisplayName.Should().Be("HMA");
        hmaResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void HmaBufferListing()
    {
        // Act
        IndicatorListing listing = Hma.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Hull Moving Average");
        listing.Uiid.Should().Be("HMA");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToHma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult hmaResult = listing.Results.SingleOrDefault(static r => r.DataName == "Hma");
        hmaResult.Should().NotBeNull();
        hmaResult?.DisplayName.Should().Be("HMA");
        hmaResult.IsReusable.Should().Be(true);
    }
}
