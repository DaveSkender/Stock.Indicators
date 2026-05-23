namespace Regression;

[TestClass, TestCategory("Regression")]
public class CciTests : RegressionTestBase<CciResult>
{
    public CciTests() : base("cci.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToCci(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => new CciList(20) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToCciHub(20).Results.IsExactly(Expected);
}
