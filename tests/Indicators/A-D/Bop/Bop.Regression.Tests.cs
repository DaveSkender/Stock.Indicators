namespace Regression;

[TestClass, TestCategory("Regression")]
public class BopTests : RegressionTestBase<BopResult>
{
    public BopTests() : base("bop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToBop().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToBopList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToBopHub(14).Results.IsExactly(Expected);
}
