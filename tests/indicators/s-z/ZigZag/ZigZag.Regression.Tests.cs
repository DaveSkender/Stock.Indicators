namespace Regression;

[TestClass, TestCategory("Regression")]
public class ZigZagTests : RegressionTestBase<ZigZagResult>
{
    public ZigZagTests() : base("zigzag-close.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToZigZag().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("BufferList implementation not available for ZigZag");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("StreamHub implementation not available for ZigZag");
}
