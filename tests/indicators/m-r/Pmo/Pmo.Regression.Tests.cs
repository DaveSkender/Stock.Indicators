namespace Regression;

[TestClass, TestCategory("Regression")]
public class PmoTests : RegressionTestBase<PmoResult>
{
    public PmoTests() : base("pmo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToPmo(35, 20, 10).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToPmoList(35, 20, 10).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToPmoHub(35, 20, 10).Results.IsExactly(Expected);
}
