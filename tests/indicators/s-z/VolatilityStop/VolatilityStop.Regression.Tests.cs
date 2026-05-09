namespace Regression;

[TestClass, TestCategory("Regression")]
public class VolatilitystopTests : RegressionTestBase<VolatilityStopResult>
{
    public VolatilitystopTests() : base("vol-stop.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToVolatilityStop().IsExactly(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToVolatilityStopList().IsExactly(Expected);

    [TestMethod]
    public override void Stream() => QuoteHub.ToVolatilityStopHub().Results.IsExactly(Expected);
}
