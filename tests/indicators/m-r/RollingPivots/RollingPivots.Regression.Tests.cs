namespace Regression;

[TestClass, TestCategory("Regression")]
public class RollingpivotsTests : RegressionTestBase<RollingPivotsResult>
{
    public RollingpivotsTests() : base("rolling-pivots.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRollingPivots(11, 9).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
