namespace Regression;

[TestClass, TestCategory("Regression")]
public class MacdTests : RegressionTestBase<MacdResult>
{
    public MacdTests() : base("macd.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMacd(12, 26, 9).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new MacdList(12, 26, 9) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToMacdHub(12, 26, 9).Results.IsExactly(Expected);
}
