namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChopTests : RegressionTestBase<ChopResult>
{
    public ChopTests() : base("chop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToChop(14).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToChopList(14).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToChopHub(14).Results.IsExactly(Expected);
}
