namespace Catalogging;

/// <summary>
/// Test class for Dynamic catalog functionality.
/// </summary>
[TestClass]
public class DynamicTests : TestBase
{
    [TestMethod]
    public void DynamicSeriesListing()
    {
        // Act
        IndicatorListing listing = MgDynamic.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("McGinley Dynamic");
        listing.Uiid.Should().Be("DYNAMIC");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToDynamic");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam kFactorParam1 = listing.Parameters.SingleOrDefault(p => p.ParameterName == "kFactor");
        kFactorParam1.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult dynamicResult = listing.Results.SingleOrDefault(r => r.DataName == "Dynamic");
        dynamicResult.Should().NotBeNull();
        dynamicResult?.DisplayName.Should().Be("McGinley Dynamic");
        dynamicResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void DynamicStreamListing()
    {
        // Act
        IndicatorListing listing = MgDynamic.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("McGinley Dynamic");
        listing.Uiid.Should().Be("DYNAMIC");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToDynamic");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam kFactorParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "kFactor");
        kFactorParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult dynamicResult = listing.Results.SingleOrDefault(r => r.DataName == "Dynamic");
        dynamicResult.Should().NotBeNull();
        dynamicResult?.DisplayName.Should().Be("McGinley Dynamic");
        dynamicResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void DynamicBufferListing()
    {
        // Act
        IndicatorListing listing = MgDynamic.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("McGinley Dynamic");
        listing.Uiid.Should().Be("DYNAMIC");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.MovingAverage);
        listing.MethodName.Should().Be("ToDynamic");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();
        IndicatorParam kFactorParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "kFactor");
        kFactorParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult dynamicResult = listing.Results.SingleOrDefault(r => r.DataName == "Dynamic");
        dynamicResult.Should().NotBeNull();
        dynamicResult?.DisplayName.Should().Be("McGinley Dynamic");
        dynamicResult.IsReusable.Should().Be(true);
    }
}
