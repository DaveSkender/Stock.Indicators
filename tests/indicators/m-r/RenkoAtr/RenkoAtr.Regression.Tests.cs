namespace Regression;

[TestClass, TestCategory("Regression")]
public class RenkoatrTests : RegressionTestBase<RenkoResult>
{
    public RenkoatrTests() : base("renko-atr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRenkoAtr().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("BufferList not implemented - requires full dataset recalculation on each add");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("StreamHub not implemented - requires full dataset recalculation on each add");
}
