namespace Utilities;

#pragma warning disable CA1805 // Do not initialize unnecessarily

[TestClass]
public class NullMathTests : TestBase
{
    private readonly double? dblPos = 100.12345;
    private readonly double? dblNeg = -200.98765;
    private readonly double? dblNul = null;
    private readonly decimal? decPos = 10.12345m;
    private readonly decimal? decNeg = -20.98765m;
    private readonly decimal? decNul = null;

    [TestMethod]
    public void AbsDouble()
    {
        dblPos.Abs().Should().Be(100.12345d);
        dblNeg.Abs().Should().Be(200.98765d);
        dblNul.Abs().Should().BeNull();
    }

    [TestMethod]
    public void RoundDecimal()
    {
        decPos.Round(2).Should().Be(10.12m);
        decNeg.Round(2).Should().Be(-20.99m);
        decNul.Round(2).Should().BeNull();

        10.12345m.Round(2).Should().Be(10.12m);
    }

    [TestMethod]
    public void RoundDouble()
    {
        dblPos.Round(2).Should().Be(100.12d);
        dblNeg.Round(2).Should().Be(-200.99d);
        dblNul.Round(2).Should().BeNull();

        100.12345d.Round(2).Should().Be(100.12d);
    }

    [TestMethod]
    public void Null2NaN()
    {
        // doubles
        dblPos.Null2NaN().Should().Be(100.12345d);
        dblNeg.Null2NaN().Should().Be(-200.98765d);
        dblNul.Null2NaN().Should().Be(double.NaN);

        // decimals » doubles
        decPos.Null2NaN().Should().Be(10.12345d);
        decNeg.Null2NaN().Should().Be(-20.98765d);
        decNul.Null2NaN().Should().Be(double.NaN);
    }

    [TestMethod]
    public void NaN2Null()
    {
        // double (nullable)
        double? dblNaNul = double.NaN;
        dblNaNul.NaN2Null().Should().BeNull();
        dblPos.NaN2Null().Should().Be(100.12345d);
        dblNeg.NaN2Null().Should().Be(-200.98765d);

        // double (non-nullable)
        double dblNaN = double.NaN;
        dblNaN.NaN2Null().Should().BeNull();
        100.12345d.NaN2Null().Should().Be(100.12345d);
        (-200.98765d).NaN2Null().Should().Be(-200.98765d);
    }
}
