namespace Regression;

[TestClass, TestCategory("Regression")]
public class BollingerbandsTests : RegressionTestBase<BollingerBandsResult>
{
    public BollingerbandsTests() : base("bb.standard.json") { }

    [TestMethod]
    public override void Series_AgainstBaseline_MatchesExactly() => Quotes.ToBollingerBands(20, 2).IsExactly(Expected);

    [TestMethod]
    public override void Buffer_AgainstBaseline_MatchesExactly() => Quotes.ToBollingerBandsList(20, 2).IsExactly(Expected);

    [TestMethod]
    public override void Stream_AgainstBaseline_MatchesExactly() => QuoteHub.ToBollingerBandsHub(20, 2).Results.IsExactly(Expected);
}
