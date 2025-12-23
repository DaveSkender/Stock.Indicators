namespace Regression;

[TestClass, TestCategory("Regression")]
public class AdxTests : RegressionTestBase<AdxResult>
{
    public AdxTests() : base("adx.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAdx(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => QuoteHub.ToAdxHub(14).Results.IsExactly(Expected);
}
