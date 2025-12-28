namespace Regression;

[TestClass, TestCategory("Regression")]
public class CmfTests : RegressionTestBase<CmfResult>
{
    public CmfTests() : base("cmf.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToCmf(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToCmfList(20).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToCmfHub(20).Results.IsExactly(Expected);
}
