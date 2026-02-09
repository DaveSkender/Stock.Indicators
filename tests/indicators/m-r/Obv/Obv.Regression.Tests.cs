namespace Regression;

[TestClass, TestCategory("Regression")]
public class ObvTests : RegressionTestBase<ObvResult>
{
    public ObvTests() : base("obv.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToObv().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToObvList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToObvHub().Results.IsExactly(Expected);
}
