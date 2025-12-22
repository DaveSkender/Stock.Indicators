namespace Regression;

[TestClass, TestCategory("Regression")]
public class SlopeTests : RegressionTestBase<SlopeResult>
{
    public SlopeTests() : base("slope.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSlope().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new SlopeList(14) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
