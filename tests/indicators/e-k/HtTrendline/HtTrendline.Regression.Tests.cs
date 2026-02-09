namespace Regression;

[TestClass, TestCategory("Regression")]
public class HtTrendlineTests : RegressionTestBase<HtlResult>
{
    public HtTrendlineTests() : base("htl.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToHtTrendline().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToHtTrendlineList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => Quotes.ToHtTrendlineHub().Results.IsExactly(Expected);
}
