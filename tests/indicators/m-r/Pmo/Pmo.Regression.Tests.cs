namespace Regression;

[TestClass, TestCategory("Regression")]
public class PmoTests : RegressionTestBase<PmoResult>
{
    public PmoTests() : base("pmo.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToPmo(35, 20, 10).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => new PmoList(35, 20, 10) { Quotes }.AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToPmoHub(35, 20, 10).Results.AssertEquals(Expected);
}
