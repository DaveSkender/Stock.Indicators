namespace Regression;

[TestClass, TestCategory("Regression")]
public class HmaTests : RegressionTestBase<HmaResult>
{
    public HmaTests() : base("hma.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToHma().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToHmaList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToHmaHub(14).Results.IsExactly(Expected);
}
