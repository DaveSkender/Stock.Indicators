namespace Regression;

[TestClass, TestCategory("Regression")]
public class MacdTests : RegressionTestBase<MacdResult>
{
    public MacdTests() : base("macd.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMacd(12, 26, 9).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
