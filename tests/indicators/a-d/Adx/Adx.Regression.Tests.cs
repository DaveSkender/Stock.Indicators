
namespace Regression;

[TestClass, TestCategory("Regression")]
public class AdxTests : RegressionTestBase<AdxResult>
{
    public AdxTests() : base("adx.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAdx(14).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => quoteHub.ToAdxHub(14).Results.AssertEquals(Expected);
}
