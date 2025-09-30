namespace Catalogging;

/// <summary>
/// Test class for Smma catalog functionality.
/// </summary>
[TestClass]
public class SmmaTests : TestBase
{
    [TestMethod]
    public void SmmaSeriesListing()
    {
        // Act
        IndicatorListing listing = Smma.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Smoothed Moving Average");
        listing.Uiid.Should().Be("SMMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToSmma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult smmaResult = listing.Results.SingleOrDefault(r => r.DataName == "Smma");
        smmaResult.Should().NotBeNull();
        smmaResult?.DisplayName.Should().Be("SMMA");
        smmaResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void SmmaStreamListing()
    {
        // Act
        IndicatorListing listing = Smma.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Smoothed Moving Average");
        listing.Uiid.Should().Be("SMMA");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToSmma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult smmaResult = listing.Results.SingleOrDefault(r => r.DataName == "Smma");
        smmaResult.Should().NotBeNull();
        smmaResult?.DisplayName.Should().Be("SMMA");
        smmaResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void SmmaBufferListing()
    {
        // Act
        IndicatorListing listing = Smma.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Smoothed Moving Average");
        listing.Uiid.Should().Be("SMMA");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToSmma");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult smmaResult = listing.Results.SingleOrDefault(r => r.DataName == "Smma");
        smmaResult.Should().NotBeNull();
        smmaResult?.DisplayName.Should().Be("SMMA");
        smmaResult.IsReusable.Should().Be(true);
    }
}
