namespace Regression;

[TestClass, TestCategory("Regression")]
public class StochTests : RegressionTestBase<StochResult>
{
    public StochTests() : base("stoch.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStoch(14, 3, 3).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => QuoteHub.ToStochHub(14, 3, 3).Results.AssertEquals(Expected);
}
