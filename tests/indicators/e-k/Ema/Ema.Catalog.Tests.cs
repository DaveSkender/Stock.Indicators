namespace Catalog;

[TestClass]
public class EmaTests : TestBase
{
    [TestMethod]
    public void EmaSeriesListing()
    {
        // Act
        var listing = Ema.SeriesListing;

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

    [TestMethod]
    public void EmaStreamListing()
    {
        // Act
        var listing = Ema.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Exponential Moving Average");
        listing.Uiid.Should().Be("EMA");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var period = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        period.Should().NotBeNull();
        period!.DisplayName.Should().Be("Lookback Period");
        period.Description.Should().Be("Number of periods for the EMA calculation");
        period.IsRequired.Should().BeTrue();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Ema");
        result.DisplayName.Should().Be("EMA");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }

    [TestMethod]
    public void EmaBufferListing()
    {
        // Act
        var listing = Ema.BufferListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Exponential Moving Average");
        listing.Uiid.Should().Be("EMA");
        listing.Style.Should().Be(Style.Buffer);
        listing.Category.Should().Be(Category.MovingAverage);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(1);

        var period = listing.Parameters.FirstOrDefault(p => p.ParameterName == "lookbackPeriods");
        period.Should().NotBeNull();
        period!.DisplayName.Should().Be("Lookback Period");
        period.Description.Should().Be("Number of periods for the EMA calculation");
        period.IsRequired.Should().BeTrue();

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Ema");
        result.DisplayName.Should().Be("EMA");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }

    [TestMethod]
    public void EmaSeriesFromCatalogMatchesDirectCall()
    {
        // Arrange
        var quotes = Quotes; // Using TestBase.Quotes
        var listing = Ema.SeriesListing;

        // Get default parameter value from catalog
        var lookbackParam = listing.Parameters.First(p => p.ParameterName == "lookbackPeriods");
        var lookbackValue = (int)lookbackParam.DefaultValue!;

        // Act - Call using catalog metadata (via method name)
        var catalogResults = quotes.ToEma(lookbackValue);

        // Act - Direct call
        var directResults = quotes.ToEma(lookbackValue);

        // Assert - Results should be identical
        catalogResults.Should().NotBeNull();
        directResults.Should().NotBeNull();
        catalogResults.Count.Should().Be(directResults.Count);

        // Compare actual values
        var catalogList = catalogResults.ToList();
        var directList = directResults.ToList();

        for (int i = 0; i < catalogList.Count; i++)
        {
            var catalogItem = catalogList[i];
            var directItem = directList[i];

            catalogItem.Timestamp.Should().Be(directItem.Timestamp);
            catalogItem.Ema.Should().Be(directItem.Ema);
        }
    }
}
