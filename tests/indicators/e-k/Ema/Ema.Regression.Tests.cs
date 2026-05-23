namespace Regression;

[TestClass, TestCategory("Regression")]
public class EmaTests : RegressionTestBase<EmaResult>
{
    public EmaTests() : base("ema.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToEma(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToEmaList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToEmaHub(20).Results.IsExactly(Expected);
}
