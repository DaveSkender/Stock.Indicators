namespace Regression;

[TestClass, TestCategory("Regression")]
public class TrTests : RegressionTestBase<TrResult>
{
    public TrTests() : base("tr.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToTr().IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToTrList().IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToTrHub().Results.IsExactly(Expected);
}
