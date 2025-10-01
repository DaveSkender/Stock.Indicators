namespace Catalogging;

/// <summary>
/// Test class for Epma catalog functionality.
/// </summary>
[TestClass]
public class EpmaTests : TestBase
{
    [TestMethod]
    public void EpmaSeriesListing()
    {
        // Act
        IndicatorListing listing = Epma.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Endpoint Moving Average");
        listing.Uiid.Should().Be("EPMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToEpma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult epmaResult = listing.Results.SingleOrDefault(r => r.DataName == "Epma");
        epmaResult.Should().NotBeNull();
        epmaResult?.DisplayName.Should().Be("EPMA");
        epmaResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void EpmaStreamListing()
    {
        // Act
        IndicatorListing listing = Epma.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Endpoint Moving Average");
        listing.Uiid.Should().Be("EPMA");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToEpma");
    }

    [TestMethod]
    public void EpmaBufferListing()
    {
        // Act
        IndicatorListing listing = Epma.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Endpoint Moving Average");
        listing.Uiid.Should().Be("EPMA");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToEpma");
    }
}
