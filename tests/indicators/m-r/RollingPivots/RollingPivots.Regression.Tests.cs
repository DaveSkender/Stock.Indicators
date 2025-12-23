namespace Regression;

[TestClass, TestCategory("Regression")]
public class RollingPivotsTests : RegressionTestBase<RollingPivotsResult>
{
    public RollingPivotsTests() : base("rolling-pivots.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRollingPivots().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
