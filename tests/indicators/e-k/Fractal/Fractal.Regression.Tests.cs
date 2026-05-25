namespace Regression;

[TestClass, TestCategory("Regression")]
public class FractalTests : RegressionTestBase<FractalResult>
{
    public FractalTests() : base("fractal.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToFractal(2).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToFractalList(2).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Quotes.ToFractalHub(2).Results.IsExactly(Expected);
}
