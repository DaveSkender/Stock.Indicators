namespace Regression;

[TestClass, TestCategory("Regression")]
public class KamaTests : RegressionTestBase<KamaResult>
{
    public KamaTests() : base("kama.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToKama(10, 2, 30).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToKamaList(10, 2, 30).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToKamaHub(10, 2, 30).Results.IsExactly(Expected);
}
