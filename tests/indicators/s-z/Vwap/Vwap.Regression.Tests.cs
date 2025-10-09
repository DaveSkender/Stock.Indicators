namespace Regression;

[TestClass, TestCategory("Regression")]
public class VwapTests : RegressionTestBase<VwapResult>
{
    public VwapTests() : base("vwap.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToVwap().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
