namespace Regression;

[TestClass, TestCategory("Regression")]
public class AdlTests : RegressionTestBase<AdlResult>
{
    public AdlTests() : base("adl.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAdl().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => QuoteHub.ToAdlHub().Results.AssertEquals(Expected);
}
