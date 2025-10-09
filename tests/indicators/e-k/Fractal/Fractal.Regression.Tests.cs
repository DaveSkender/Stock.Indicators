using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class FractalTests : RegressionTestBase<FractalResult>
{
    public FractalTests() : base("fractal.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToFractal(2).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
