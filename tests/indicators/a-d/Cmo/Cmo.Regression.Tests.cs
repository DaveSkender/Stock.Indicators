namespace Regression;

[TestClass, TestCategory("Regression")]
public class CmoTests : RegressionTestBase<CmoResult>
{
    public CmoTests() : base("cmo.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToCmo(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToCmoList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToCmoHub(14).Results.IsExactly(Expected);
}
