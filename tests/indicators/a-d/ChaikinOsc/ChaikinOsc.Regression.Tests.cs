namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChaikinoscTests : RegressionTestBase<ChaikinOscResult>
{
    public ChaikinoscTests() : base("chaikin-osc.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToChaikinOsc(3, 10).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToChaikinOscList(3, 10).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToChaikinOscHub(3, 10).Results.IsExactly(Expected);
}
