namespace Catalog;

[TestClass]
public class TrTests : TestBase
{
    [TestMethod]
    public void TrSeriesListing()
    {
        // Act
        var listing = Tr.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("True Range");
        listing.Uiid.Should().Be("TR");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceCharacteristic);

        (listing.Parameters?.Count ?? 0).Should().Be(0);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Tr");
        result.DisplayName.Should().Be("True Range");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }

    [TestMethod]
    public void TrStreamListing()
    {
        // Act
        var listing = Tr.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("True Range");
        listing.Uiid.Should().Be("TR");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.PriceCharacteristic);

        (listing.Parameters?.Count ?? 0).Should().Be(0);

        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(1);

        var result = listing.Results[0];
        result.DataName.Should().Be("Tr");
        result.DisplayName.Should().Be("True Range");
        result.DataType.Should().Be(ResultType.Default);
        result.IsDefault.Should().BeTrue();
    }
}
