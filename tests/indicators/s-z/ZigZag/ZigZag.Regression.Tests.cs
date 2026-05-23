namespace Regression;

[TestClass, TestCategory("Regression")]
public class ZigZagTests : RegressionTestBase<ZigZagResult>
{
    public ZigZagTests() : base("zigzag-close.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToZigZag().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Assert.Inconclusive("Test not yet implemented");
    // TODO: BufferList implementation not available for ZigZag

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => Assert.Inconclusive("Test not yet implemented");
    // TODO: StreamHub implementation not available for ZigZag
}
