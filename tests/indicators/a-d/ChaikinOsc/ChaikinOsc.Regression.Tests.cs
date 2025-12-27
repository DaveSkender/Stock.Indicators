namespace Regression;

[TestClass, TestCategory("Regression")]
public class ChaikinoscTests : RegressionTestBase<ChaikinOscResult>
{
    public ChaikinoscTests() : base("chaikin-osc.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToChaikinOsc(3, 10).IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => new ChaikinOscList(3, 10) { Quotes }.IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToChaikinOscHub(3, 10).Results.IsExactly(Expected);
}
