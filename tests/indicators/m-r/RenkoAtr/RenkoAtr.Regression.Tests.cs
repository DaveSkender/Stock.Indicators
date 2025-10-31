namespace Regression;

[TestClass, TestCategory("Regression")]
public class RenkoatrTests : RegressionTestBase<RenkoResult>
{
    public RenkoatrTests() : base("renko-atr.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToRenkoAtr().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToRenkoAtrList().AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
