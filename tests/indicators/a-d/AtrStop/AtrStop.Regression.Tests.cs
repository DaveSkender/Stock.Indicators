namespace Regression;

[TestClass, TestCategory("Regression")]
public class AtrstopTests : RegressionTestBase<AtrStopResult>
{
    public AtrstopTests() : base("atr-stop.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToAtrStop(21, 3).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToAtrStopList(21, 3).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToAtrStopHub(21, 3).Results.IsExactly(Expected);
}
