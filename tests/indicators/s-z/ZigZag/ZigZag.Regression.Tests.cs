namespace Regression;

[TestClass, TestCategory("Regression")]
public class ZigZagTests : RegressionTestBase<ZigZagResult>
{
    public ZigZagTests() : base("zigzag-close.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToZigZag().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new ZigZagList() { Quotes }.AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
