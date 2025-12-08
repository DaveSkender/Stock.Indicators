namespace Utilities;

// TODO: is ToPrecision code used or useful as base code or just in test?
//       and is it redundant with AssertEquals precision profiles?

[TestClass]
[TestCategory("Utilities")]
public class NumericalPrecision
{
    [TestMethod]
    public void ToPrecision_WithMaxDigits_RoundsCorrectly()
    {
        const double value = 123.45678901234567;
        double sut = value.ToPrecision(17);
        sut.Should().Be(123.45678901234567);
    }

    [TestMethod]
    public void ToPrecision_OverMaxDigits_RoundsCorrectly()
    {
        const double value = 123.456789012345678;
        double sut = value.ToPrecision(17);
        sut.Should().Be(123.45678901234568);
    }

    [TestMethod]
    public void ToPrecision_WithSmallDigits_RoundsUp()
    {
        const double value = 123.4567890;
        double sut = value.ToPrecision(6);
        sut.Should().Be(123.457);
    }

    [TestMethod]
    public void ToPrecision_WithSmallDigits_RoundsMidDown()
    {
        const double value = 123.4565;  // note: rounds down on midpoint
        double sut = value.ToPrecision(6);
        sut.Should().Be(123.456);
    }

    [TestMethod]
    public void ToPrecision_WithSmallDigits_RoundsDown()
    {
        const double value = 123.4564999;
        double sut = value.ToPrecision(6);
        sut.Should().Be(123.456);
    }

    [TestMethod]
    public void ToPrecision_NearZero_RoundsCorrectly()
    {
        const double value = 0.00000000000001421085;
        double sut = value.ToPrecision(13);
        sut.Should().Be(0.0d);
    }

    [TestMethod]
    public void ToPrecision_HandlesZero()
    {
        const double value = 0.0;
        double sut = value.ToPrecision();
        sut.Should().Be(0.0);
    }

    [TestMethod]
    public void ToPrecision_HandlesNaN()
    {
        const double value = double.NaN;
        double sut = value.ToPrecision();
        double.IsNaN(sut).Should().BeTrue();
    }

    [TestMethod]
    public void ToPrecision_HandlesInfinity()
    {
        const double value = double.PositiveInfinity;
        double sut = value.ToPrecision();
        double.IsPositiveInfinity(sut).Should().BeTrue();
    }

    [TestMethod]
    public void ToPrecision_ThrowsOnInvalidDigits()
    {
        const double value = 123.4567890123456789;

        Action act1 = () => value.ToPrecision(0);
        Action act2 = () => value.ToPrecision(18);
        act1.Should().Throw<ArgumentOutOfRangeException>();
        act2.Should().Throw<ArgumentOutOfRangeException>();
    }
}
