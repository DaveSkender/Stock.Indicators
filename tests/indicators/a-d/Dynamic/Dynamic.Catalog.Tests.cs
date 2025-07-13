namespace Catalog;

[TestClass]
public class DynamicTests : TestBase
{
    [TestMethod]
    public void DynamicSeriesListing()
    {
        // Act
        var listing = MgDynamic.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("McGinley Dynamic");
        listing.Uiid.Should().Be("DYNAMIC");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        var lookbackPeriods = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        lookbackPeriods.Should().NotBeNull();
        lookbackPeriods!.DisplayName.Should().Be("Lookback Periods");
        lookbackPeriods.Description.Should().Be("Number of periods for the McGinley Dynamic calculation");
        lookbackPeriods.IsRequired.Should().BeTrue();

        var kFactor = listing.Parameters.FirstOrDefault(p => p.ParameterName == "kFactor");
        kFactor.Should().NotBeNull();
        kFactor!.DisplayName.Should().Be("K Factor");
        kFactor.Description.Should().Be("Smoothing factor for the calculation");
        kFactor.IsRequired.Should().BeFalse();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Dynamic");
        result.DisplayName.Should().Be("McGinley Dynamic");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }
}
