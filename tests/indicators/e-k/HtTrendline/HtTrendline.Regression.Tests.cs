namespace Regression;

[TestClass, TestCategory("Regression")]
public class HtTrendlineTests : RegressionTestBase<HtlResult>
{
    public HtTrendlineTests() : base("htl.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHtTrendline().AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToHtlList().AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
