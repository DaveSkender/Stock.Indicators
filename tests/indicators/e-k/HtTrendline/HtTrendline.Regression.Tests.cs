namespace Regression;

[TestClass, TestCategory("Regression")]
public class HtTrendlineTests : RegressionTestBase<HtlResult>
{
    public HtTrendlineTests() : base("htl.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHtTrendline().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Assert.Inconclusive("Buffer implementation not yet available");

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
