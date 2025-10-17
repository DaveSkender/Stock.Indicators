namespace Catalogging;

/// <summary>
/// Test class for Mfi catalog functionality.
/// </summary>
[TestClass]
public class MfiTests : TestBase
{
    [TestMethod]
    public void MfiSeriesListing()
    {
        // Act
        IndicatorListing listing = Mfi.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Money Flow Index (MFI)");
        listing.Uiid.Should().Be("MFI");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.VolumeBased);
        listing.MethodName.Should().Be("ToMfi");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult mfiResult = listing.Results.SingleOrDefault(r => r.DataName == "Mfi");
        mfiResult.Should().NotBeNull();
        mfiResult?.DisplayName.Should().Be("MFI");
        mfiResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void MfiStreamListing()
    {
        // Act
        IndicatorListing listing = Mfi.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Money Flow Index (MFI)");
        listing.Uiid.Should().Be("MFI");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.VolumeBased);
        listing.MethodName.Should().Be("ToMfi");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult mfiResult = listing.Results.SingleOrDefault(r => r.DataName == "Mfi");
        mfiResult.Should().NotBeNull();
        mfiResult?.DisplayName.Should().Be("MFI");
        mfiResult.IsReusable.Should().Be(true);
    }

    [TestMethod]
    public void MfiBufferListing()
    {
        // Act
        IndicatorListing listing = Mfi.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Money Flow Index (MFI)");
        listing.Uiid.Should().Be("MFI");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.VolumeBased);
        listing.MethodName.Should().Be("ToMfi");

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        IndicatorParam lookbackPeriodsParam = listing.Parameters.SingleOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriodsParam.Should().NotBeNull();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        IndicatorResult mfiResult = listing.Results.SingleOrDefault(r => r.DataName == "Mfi");
        mfiResult.Should().NotBeNull();
        mfiResult?.DisplayName.Should().Be("MFI");
        mfiResult.IsReusable.Should().Be(true);
    }
}
