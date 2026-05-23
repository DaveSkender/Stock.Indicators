namespace Regression;

[TestClass, TestCategory("Regression")]
public class UltimateTests : RegressionTestBase<UltimateResult>
{
    public UltimateTests() : base("uo.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToUltimate(7, 14, 28).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToUltimateList(7, 14, 28).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToUltimateHub(7, 14, 28).Results.IsExactly(Expected);
}
