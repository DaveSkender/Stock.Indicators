namespace Catalogging;

/// <summary>
/// Test class for Trix catalog functionality.
/// </summary>
[TestClass]
public class TrixTests : TestBase
{
    [TestMethod]
    public void TrixSeriesListing()
    {
        // Act
        IndicatorListing listing = Trix.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Triple Exponential Moving Average Oscillator");
        listing.Uiid.Should().Be("TRIX");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToTrix");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult trixResult = listing.Results.SingleOrDefault(static r => r.DataName == "Trix");
        trixResult.Should().NotBeNull();
        trixResult?.DisplayName.Should().Be("TRIX");
        trixResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void TrixBufferListing()
    {
        // Act
        IndicatorListing listing = Trix.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Triple Exponential Moving Average Oscillator");
        listing.Uiid.Should().Be("TRIX");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToTrix");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult trixResult = listing.Results.SingleOrDefault(static r => r.DataName == "Trix");
        trixResult.Should().NotBeNull();
        trixResult?.DisplayName.Should().Be("TRIX");
        trixResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void TrixStreamListing()
    {
        // Act
        IndicatorListing listing = Trix.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Triple Exponential Moving Average Oscillator");
        listing.Uiid.Should().Be("TRIX");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToTrix");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult trixResult = listing.Results.SingleOrDefault(static r => r.DataName == "Trix");
        trixResult.Should().NotBeNull();
        trixResult?.DisplayName.Should().Be("TRIX");
        trixResult.IsReusable.Should().Be(true);
    }
}
