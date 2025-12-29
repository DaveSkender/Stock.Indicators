namespace Regression;

[TestClass, TestCategory("Regression")]
public class StochrsiTests : RegressionTestBase<StochRsiResult>
{
    public StochrsiTests() : base("stoch-rsi.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToStochRsi().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToStochRsiList(14, 14, 3, 1).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToStochRsiHub().Results.IsExactly(Expected);
}
