namespace Catalog;

[TestClass]
public class AtrStopTests : TestBase
{
    [TestMethod]
    public void AtrStopListing()
    {
        // Act
        var listing = AtrStop.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("ATR Trailing Stop");
        listing.Uiid.Should().Be("ATR-STOP");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.PriceTrend);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(3);

        // lookback periods parameter
        var lookbackParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "lookbackPeriods");

        lookbackParam.Should().NotBeNull();
        lookbackParam!.DisplayName.Should().Be("Lookback Periods");
        lookbackParam.DataType.Should().Be("Int32");
        lookbackParam.DefaultValue.Should().Be(21);
        lookbackParam.Minimum.Should().Be(1);
        lookbackParam.Maximum.Should().Be(50);
        lookbackParam.EnumOptions.Should().BeNull();

        // multiplier parameter
        var multiplierParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "multiplier");

        multiplierParam.Should().NotBeNull();
        multiplierParam!.DisplayName.Should().Be("Multiplier");
        multiplierParam.DataType.Should().Be("Double");
        multiplierParam.DefaultValue.Should().Be(3.0);
        multiplierParam.Minimum.Should().Be(0.1);
        multiplierParam.Maximum.Should().Be(10.0);
        multiplierParam.EnumOptions.Should().BeNull();

        // end type enum parameter
        var endTypeParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "endType");

        endTypeParam.Should().NotBeNull();
        endTypeParam!.DisplayName.Should().Be("End Type");
        endTypeParam.DataType.Should().Be("enum");
        endTypeParam.DefaultValue.Should().Be(0); // EndType.Close = 0
        endTypeParam.Minimum.Should().BeNull(); // Enum parameters should not have min/max values
        endTypeParam.Maximum.Should().BeNull(); // Enum parameters should not have min/max values
        endTypeParam.EnumOptions.Should().NotBeNull();
        endTypeParam.EnumOptions.Should().HaveCount(2);
        endTypeParam.EnumOptions![0].Should().Be("Close");
        endTypeParam.EnumOptions[1].Should().Be("HighLow");

        // results
        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(4);

        // check that AtrStop is the default result
        var defaultResult = listing.Results.FirstOrDefault(r => r.IsReusable);
        defaultResult.Should().NotBeNull();
        defaultResult!.DataName.Should().Be("AtrStop");
        defaultResult.DisplayName.Should().Be("ATR Trailing Stop");

        // check other results exist
        listing.Results.Should().Contain(r => r.DataName == "BuyStop");
        listing.Results.Should().Contain(r => r.DataName == "SellStop");
        listing.Results.Should().Contain(r => r.DataName == "Atr");
    }
}
