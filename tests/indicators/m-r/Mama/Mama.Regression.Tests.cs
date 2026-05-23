namespace Regression;

[TestClass, TestCategory("Regression")]
public class MamaTests : RegressionTestBase<MamaResult>
{
    public MamaTests() : base("mama.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToMama(0.5, 0.05).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToMamaList(0.5, 0.05).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToMamaHub(0.5, 0.05).Results.IsExactly(Expected);
}
