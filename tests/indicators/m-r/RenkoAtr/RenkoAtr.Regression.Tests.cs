namespace Regression;

[TestClass, TestCategory("Regression")]
public class RenkoAtrTests : RegressionTestBase<RenkoResult>
{
    public RenkoAtrTests() : base("renko-atr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRenkoAtr().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Test not yet implemented");
    // TODO: BufferList implementation not available for RenkoAtr

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Test not yet implemented");
    // TODO: StreamHub implementation not available for RenkoAtr
}
