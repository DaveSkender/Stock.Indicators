namespace Regression;

[TestClass, TestCategory("Regression")]
public class EmaTests : RegressionTestBase<EmaResult>
{
    public EmaTests() : base("ema.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToEma(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => QuoteHub.ToEmaHub(20).Results.AssertEquals(Expected);
}
