namespace Regression;

[TestClass, TestCategory("Regression")]
public class MarubozuTests : RegressionTestBase<CandleResult>
{
    public MarubozuTests() : base("marubozu.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToMarubozu().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToMarubozuList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToMarubozuHub().Results.IsExactly(Expected);
}
