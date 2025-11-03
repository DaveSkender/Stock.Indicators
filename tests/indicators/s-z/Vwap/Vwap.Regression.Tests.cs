namespace Regression;

[TestClass, TestCategory("Regression")]
public class VwapTests : RegressionTestBase<VwapResult>
{
    public VwapTests() : base("vwap.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToVwap().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToVwapList().AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToVwapHub().Results.AssertEquals(Expected);
}
