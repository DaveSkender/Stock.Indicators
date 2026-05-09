namespace Regression;

[TestClass, TestCategory("Regression")]
public class CciTests : RegressionTestBase<CciResult>
{
    public CciTests() : base("cci.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToCci(20).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new CciList(20) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToCciHub(20).Results.IsExactly(Expected);
}
