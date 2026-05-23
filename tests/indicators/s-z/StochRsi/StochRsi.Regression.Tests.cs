namespace Regression;

[TestClass, TestCategory("Regression")]
public class StochrsiTests : RegressionTestBase<StochRsiResult>
{
    public StochrsiTests() : base("stoch-rsi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToStochRsi().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToStochRsiList(14, 14, 3, 1).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToStochRsiHub().Results.IsExactly(Expected);
}
