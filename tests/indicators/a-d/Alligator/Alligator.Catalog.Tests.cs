namespace Catalog;

[TestClass]
public class AlligatorTests : TestBase
{
    [TestMethod]
    public void AlligatorSeriesListing()
    {
        // Act
        var listing = Alligator.SeriesListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Williams Alligator");
        listing.Uiid.Should().Be("ALLIGATOR");
        listing.Style.Should().Be(Style.Series);
        listing.Category.Should().Be(Category.Oscillator);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(6);

        // jaw periods parameter
        var jawPeriodsParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "jawPeriods");

        jawPeriodsParam.Should().NotBeNull();
        jawPeriodsParam!.DisplayName.Should().Be("Jaw Periods");
        jawPeriodsParam.DataType.Should().Be("Int32");
        jawPeriodsParam.DefaultValue.Should().Be(13);
        jawPeriodsParam.Minimum.Should().Be(2);
        jawPeriodsParam.Maximum.Should().Be(250);
        jawPeriodsParam.IsRequired.Should().BeTrue();

        // jaw offset parameter
        var jawOffsetParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "jawOffset");

        jawOffsetParam.Should().NotBeNull();
        jawOffsetParam!.DisplayName.Should().Be("Jaw Offset");
        jawOffsetParam.DataType.Should().Be("Int32");
        jawOffsetParam.DefaultValue.Should().Be(8);
        jawOffsetParam.Minimum.Should().Be(1);
        jawOffsetParam.Maximum.Should().Be(50);
        jawOffsetParam.IsRequired.Should().BeTrue();

        // teeth periods parameter
        var teethPeriodsParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "teethPeriods");

        teethPeriodsParam.Should().NotBeNull();
        teethPeriodsParam!.DisplayName.Should().Be("Teeth Periods");
        teethPeriodsParam.DataType.Should().Be("Int32");
        teethPeriodsParam.DefaultValue.Should().Be(8);
        teethPeriodsParam.Minimum.Should().Be(2);
        teethPeriodsParam.Maximum.Should().Be(250);
        teethPeriodsParam.IsRequired.Should().BeTrue();

        // teeth offset parameter
        var teethOffsetParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "teethOffset");

        teethOffsetParam.Should().NotBeNull();
        teethOffsetParam!.DisplayName.Should().Be("Teeth Offset");
        teethOffsetParam.DataType.Should().Be("Int32");
        teethOffsetParam.DefaultValue.Should().Be(5);
        teethOffsetParam.Minimum.Should().Be(1);
        teethOffsetParam.Maximum.Should().Be(50);
        teethOffsetParam.IsRequired.Should().BeTrue();

        // lips periods parameter
        var lipsPeriodsParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "lipsPeriods");

        lipsPeriodsParam.Should().NotBeNull();
        lipsPeriodsParam!.DisplayName.Should().Be("Lips Periods");
        lipsPeriodsParam.DataType.Should().Be("Int32");
        lipsPeriodsParam.DefaultValue.Should().Be(5);
        lipsPeriodsParam.Minimum.Should().Be(2);
        lipsPeriodsParam.Maximum.Should().Be(250);
        lipsPeriodsParam.IsRequired.Should().BeTrue();

        // lips offset parameter
        var lipsOffsetParam = listing.Parameters
            .FirstOrDefault(p => p.ParameterName == "lipsOffset");

        lipsOffsetParam.Should().NotBeNull();
        lipsOffsetParam!.DisplayName.Should().Be("Lips Offset");
        lipsOffsetParam.DataType.Should().Be("Int32");
        lipsOffsetParam.DefaultValue.Should().Be(3);
        lipsOffsetParam.Minimum.Should().Be(1);
        lipsOffsetParam.Maximum.Should().Be(50);
        lipsOffsetParam.IsRequired.Should().BeTrue();

        // results
        listing.Results.Should().NotBeNull();
        listing.Results.Should().HaveCount(3);

        // check that Lips is the default result
        var defaultResult = listing.Results.FirstOrDefault(r => r.IsDefault);
        defaultResult.Should().NotBeNull();
        defaultResult!.DataName.Should().Be("Lips");
        defaultResult.DisplayName.Should().Be("Lips");

        // check other results exist
        listing.Results.Should().Contain(r => r.DataName == "Jaw");
        listing.Results.Should().Contain(r => r.DataName == "Teeth");
    }

    [TestMethod]
    public void AlligatorStreamListing()
    {
        // Act
        var listing = Alligator.StreamListing;

        // Assert
        listing.Should().NotBeNull();
        listing.Name.Should().Be("Williams Alligator");
        listing.Uiid.Should().Be("ALLIGATOR");
        listing.Style.Should().Be(Style.Stream);
        listing.Category.Should().Be(Category.Oscillator);

        listing.Parameters.Should().NotBeNull();
        listing.Parameters.Should().HaveCount(6);

        // check that Lips is the default result
        var defaultResult = listing.Results.FirstOrDefault(r => r.IsDefault);
        defaultResult.Should().NotBeNull();
        defaultResult!.DataName.Should().Be("Lips");
        defaultResult.DisplayName.Should().Be("Lips");

        // check other results exist
        listing.Results.Should().Contain(r => r.DataName == "Jaw");
        listing.Results.Should().Contain(r => r.DataName == "Teeth");
    }
}