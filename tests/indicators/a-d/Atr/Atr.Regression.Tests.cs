namespace Regression;

[TestClass, TestCategory("Regression")]
public class AtrTests : RegressionTestBase<AtrResult>
{
    public AtrTests() : base("atr.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToAtr(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToAtrList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToAtrHub(14).Results.IsExactly(Expected);
}
