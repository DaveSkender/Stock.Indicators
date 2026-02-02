namespace Regression;

[TestClass, TestCategory("Regression")]
public class FractalTests : RegressionTestBase<FractalResult>
{
    public FractalTests() : base("fractal.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToFractal(2).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToFractalList(2).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToFractalHub(2).Results.IsExactly(Expected);
}
