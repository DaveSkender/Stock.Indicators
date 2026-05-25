namespace Regression;

[TestClass, TestCategory("Regression")]
public class DemaTests : RegressionTestBase<DemaResult>
{
    public DemaTests() : base("dema.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToDema().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToDemaList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToDemaHub(14).Results.IsExactly(Expected);
}
