namespace Regression;

[TestClass, TestCategory("Regression")]
public class BollingerbandsTests : RegressionTestBase<BollingerBandsResult>
{
    public BollingerbandsTests() : base("bb.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToBollingerBands(20, 2).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToBollingerBandsList(20, 2).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToBollingerBandsHub(20, 2).Results.IsExactly(Expected);
}
