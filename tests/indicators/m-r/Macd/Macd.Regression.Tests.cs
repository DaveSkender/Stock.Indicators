namespace Regression;

[TestClass, TestCategory("Regression")]
public class MacdTests : RegressionTestBase<MacdResult>
{
    public MacdTests() : base("macd.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToMacd(12, 26, 9).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToMacdList(12, 26, 9).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToMacdHub(12, 26, 9).Results.IsExactly(Expected);
}
