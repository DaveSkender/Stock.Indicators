namespace Regression;

[TestClass, TestCategory("Regression")]
public class ZigZagTests : RegressionTestBase<ZigZagResult>
{
    public ZigZagTests() : base("zigzag-close.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToZigZag().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
