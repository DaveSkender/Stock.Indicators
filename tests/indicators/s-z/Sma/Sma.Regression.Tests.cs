namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmaTests : RegressionTestBase<SmaResult>
{
    public SmaTests() : base("sma.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToSma(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToSmaList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToSmaHub(20).Results.IsExactly(Expected);
}
