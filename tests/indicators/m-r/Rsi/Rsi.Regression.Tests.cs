namespace Regression;

[TestClass, TestCategory("Regression")]
public class RsiTests : RegressionTestBase<RsiResult>
{
    public RsiTests() : base("rsi.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToRsi(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToRsiList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToRsiHub(14).Results.IsExactly(Expected);
}
