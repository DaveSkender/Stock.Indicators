namespace Catalog;

[TestClass]
public class RenkoTests : TestBase
{
    [TestMethod]
    public void RenkoSeriesListing()
    {
        // Act
        var listing = Renko.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Renko Chart");
        listing.Uiid.Should().Be("RENKO");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceTransform);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        // brick size parameter
        var brickSizeParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "brickSize");

        brickSizeParam.Should().NotBeNull();
        brickSizeParam!.DisplayName.Should().Be("Brick Size");
        brickSizeParam.Description.Should().Be("The size of each Renko brick");
        brickSizeParam.DataType.Should().Be("Double");
        brickSizeParam.DefaultValue.Should().Be(1.0);
        brickSizeParam.Minimum.Should().Be(0.001);
        brickSizeParam.Maximum.Should().Be(1000000.0);
        brickSizeParam.IsRequired.Should().BeTrue();

        // end type enum parameter
        var endTypeParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "endType");

        endTypeParam.Should().NotBeNull();
        endTypeParam!.DisplayName.Should().Be("End Type");
        endTypeParam.Description.Should().Be("The price candle end type to use as the brick threshold");
        endTypeParam.DataType.Should().Be("enum");
        endTypeParam.DefaultValue.Should().Be(0); // EndType.Close = 0
        endTypeParam.Minimum.Should().BeNull(); // Enum parameters should not have min/max values
        endTypeParam.Maximum.Should().BeNull(); // Enum parameters should not have min/max values
        endTypeParam.EnumOptions.Should().NotBeNull();
        endTypeParam.EnumOptions.Should().HaveCount(2);
        endTypeParam.EnumOptions![0].Should().Be("Close");
        endTypeParam.EnumOptions[1].Should().Be("HighLow");
        endTypeParam.IsRequired.Should().BeFalse();

        // results
        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(6);

        // check that Close is the default result
        var defaultResult = listing.Results.FirstOrDefault(r => r.IsReusable);
        defaultResult.Should().NotBeNull();
        defaultResult!.DataName.Should().Be("Close");
        defaultResult.DisplayName.Should().Be("Close");

        // check other results exist
        listing.Results.Should().Contain(r => r.DataName == "Open");
        listing.Results.Should().Contain(r => r.DataName == "High");
        listing.Results.Should().Contain(r => r.DataName == "Low");
        listing.Results.Should().Contain(r => r.DataName == "Volume");
        listing.Results.Should().Contain(r => r.DataName == "IsUp");
    }

    [TestMethod]
    public void RenkoStreamListing()
    {
        // Act
        var listing = Renko.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Renko Chart");
        listing.Uiid.Should().Be("RENKO");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.PriceTransform);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(2);

        // results
        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(6);

        // check that Close is the default result
        var defaultResult = listing.Results.FirstOrDefault(r => r.IsReusable);
        defaultResult.Should().NotBeNull();
        defaultResult!.DataName.Should().Be("Close");
        defaultResult.DisplayName.Should().Be("Close");

        // check other results exist
        listing.Results.Should().Contain(r => r.DataName == "Open");
        listing.Results.Should().Contain(r => r.DataName == "High");
        listing.Results.Should().Contain(r => r.DataName == "Low");
        listing.Results.Should().Contain(r => r.DataName == "Volume");
        listing.Results.Should().Contain(r => r.DataName == "IsUp");
    }
}
