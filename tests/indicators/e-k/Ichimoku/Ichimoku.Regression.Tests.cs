namespace Regression;

[TestClass, TestCategory("Regression")]
public class IchimokuTests : RegressionTestBase<IchimokuResult>
{
    public IchimokuTests() : base("ichimoku.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToIchimoku(9, 26, 52).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToIchimokuList(9, 26, 52).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToIchimokuHub(9, 26, 52).Results.IsExactly(Expected);
}
