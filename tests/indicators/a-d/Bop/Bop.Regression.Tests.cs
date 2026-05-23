namespace Regression;

[TestClass, TestCategory("Regression")]
public class BopTests : RegressionTestBase<BopResult>
{
    public BopTests() : base("bop.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToBop().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToBopList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToBopHub(14).Results.IsExactly(Expected);
}
