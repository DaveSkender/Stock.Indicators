namespace Regression;

[TestClass, TestCategory("Regression")]
public class AroonTests : RegressionTestBase<AroonResult>
{
    public AroonTests() : base("aroon.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAroon(25).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => QuoteHub.ToAroonHub(25).Results.AssertEquals(Expected);
}
