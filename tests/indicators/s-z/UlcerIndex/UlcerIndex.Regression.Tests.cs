namespace Regression;

[TestClass, TestCategory("Regression")]
public class UlcerindexTests : RegressionTestBase<UlcerIndexResult>
{
    public UlcerindexTests() : base("ulcer.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToUlcerIndex(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToUlcerIndexList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToUlcerIndexHub(14).Results.IsExactly(Expected);
}
