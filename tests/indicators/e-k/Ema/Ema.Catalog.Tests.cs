namespace Catalog;

[TestClass]
public class EmaTests : TestBase
{
    [TestMethod]
    public void EmaListing()
    {
        // Act
        var listing = Ema.Listing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Exponential Moving Average");
        listing.Uiid.Should().Be("EMA");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        var period = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        period.Should().NotBeNull();
        period!.DisplayName.Should().Be("Lookback Period");
        period.Description.Should().Be("Number of periods for the EMA calculation");
        period.IsRequired.Should().BeTrue();

        var smoothingFactor = listing.Parameters.FirstOrDefault(p => p.ParameterName == "smoothingFactor");
        smoothingFactor.Should().NotBeNull();
        smoothingFactor!.DisplayName.Should().Be("Smoothing Factor");
        smoothingFactor.Description.Should().Be("Optional custom smoothing factor");
        smoothingFactor.IsRequired.Should().BeFalse();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Ema");
        result.DisplayName.Should().Be("EMA");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }
}
