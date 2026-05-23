namespace Regression;

[TestClass, TestCategory("Regression")]
public class CmfTests : RegressionTestBase<CmfResult>
{
    public CmfTests() : base("cmf.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToCmf(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToCmfList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToCmfHub(20).Results.IsExactly(Expected);
}
