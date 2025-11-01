namespace Catalogging;

/// <summary>
/// Test class for Ultimate catalog functionality.
/// </summary>
[TestClass]
public class UltimateTests : TestBase
{
    [TestMethod]
    public void UltimateSeriesListing()
    {
        // Act
        IndicatorListing listing = Ultimate.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Ultimate Oscillator");
        listing.Uiid.Should().Be("UO");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToUltimate");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        IndicatorParam shortPeriodsParam = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "shortPeriods");
        shortPeriodsParam.Should().NotBeNull();
        IndicatorParam middlePeriodsParam1 = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "middlePeriods");
        middlePeriodsParam1.Should().NotBeNull();
        IndicatorParam longPeriodsParam2 = listing.Parameters.SingleOrDefault(static p => p.ParameterName == "longPeriods");
        longPeriodsParam2.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult ultimateResult = listing.Results.SingleOrDefault(static r => r.DataName == "Ultimate");
        ultimateResult.Should().NotBeNull();
        ultimateResult?.DisplayName.Should().Be("Ultimate Oscillator");
        ultimateResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void UltimateBufferListing()
    {
        // Act
        IndicatorListing listing = Ultimate.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Ultimate Oscillator");
        listing.Uiid.Should().Be("UO");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToUltimate");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult ultimateResult = listing.Results.SingleOrDefault(static r => r.DataName == "Ultimate");
        ultimateResult.Should().NotBeNull();
        ultimateResult?.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void UltimateStreamListing()
    {
        // Act
        IndicatorListing listing = Ultimate.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Ultimate Oscillator");
        listing.Uiid.Should().Be("UO");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.Oscillator);
        listing.MethodName.Should().Be("ToUltimate");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult ultimateResult = listing.Results.SingleOrDefault(static r => r.DataName == "Ultimate");
        ultimateResult.Should().NotBeNull();
        ultimateResult?.IsReusable.Should().Be(true);
    }
}
