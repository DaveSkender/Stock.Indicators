namespace Regression;

[TestClass, TestCategory("Regression")]
public class MarubozuTests : RegressionTestBase<CandleResult>
{
    public MarubozuTests() : base("marubozu.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToMarubozu().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToMarubozuList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToMarubozuHub().Results.IsExactly(Expected);
}
