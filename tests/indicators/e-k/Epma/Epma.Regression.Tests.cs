namespace Regression;

[TestClass, TestCategory("Regression")]
public class EpmaTests : RegressionTestBase<EpmaResult>
{
    public EpmaTests() : base("epma.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToEpma().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToEpmaList(10).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToEpmaHub(10).Results.IsExactly(Expected);
}
