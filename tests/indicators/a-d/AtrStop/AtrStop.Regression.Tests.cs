namespace Regression;

[TestClass, TestCategory("Regression")]
public class AtrstopTests : RegressionTestBase<AtrStopResult>
{
    public AtrstopTests() : base("atr-stop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToAtrStop(21, 3).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToAtrStopList(21, 3).IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToAtrStopHub(21, 3).Results.IsExactly(Expected);
}
